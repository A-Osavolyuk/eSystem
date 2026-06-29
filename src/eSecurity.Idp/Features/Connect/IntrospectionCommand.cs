using System.Text.Json;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authorization.Token;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSecurity.Idp.Security.Cryptography.Tokens;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Security.Authorization.OAuth.Introspection;

namespace eSecurity.Idp.Features.Connect;

public sealed class IntrospectionCommand : IRequest<Result>
{
    [FromForm(Name = "token")]
    public string Token { get; init; } = null!;
    
    [FromForm(Name = "token_type_hint")]
    public TokenTypeHint? TokenTypeHint { get; init; }
}

public sealed class IntrospectionCommandHandler(
    ITokenManager tokenManager,
    IUserQueryService userQueryService,
    IHasherProvider hasherProvider,
    IOptions<TokenConfigurations> options,
    IClientQueryService clientQueryService,
    ISessionQueryService sessionQueryService) : IRequestHandler<IntrospectionCommand, Result>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly ISessionQueryService _sessionQueryService = sessionQueryService;
    private readonly TokenConfigurations _configurations = options.Value;

    public async Task<Result> Handle(IntrospectionCommand request, CancellationToken cancellationToken)
    {
        OpaqueTokenType? opaqueTokenType = request.TokenTypeHint switch
        {
            TokenTypeHint.AccessToken => OpaqueTokenType.AccessToken,
            TokenTypeHint.RefreshToken => OpaqueTokenType.RefreshToken,
            _ => null
        };

        var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
        var incomingHash = hasher.Hash(request.Token);

        var token = opaqueTokenType.HasValue
            ? await _tokenManager.FindByHashAsync(incomingHash, opaqueTokenType.Value, cancellationToken)
            : await _tokenManager.FindByHashAsync(incomingHash, cancellationToken);

        if (token is null || !token.IsValid || token.TokenType is OpaqueTokenType.LoginToken)
            return Results.Success(SuccessCodes.Ok, IntrospectionResponse.Fail());

        var tokenType = token.TokenType switch
        {
            OpaqueTokenType.AccessToken => TokenType.AccessToken,
            OpaqueTokenType.RefreshToken => TokenType.RefreshToken,
            _ => throw new NotSupportedException("Unsupported token type")
        };

        var clientAudiences = await _clientQueryService.GetSupportedAudiencesAsync(token.Client.Id, cancellationToken);
        var response = new IntrospectionResponse
        {
            Active = true,
            TokenType = tokenType,
            ClientId = token.Client.Id,
            Issuer = _configurations.Issuer,
            Audience = JsonSerializer.Serialize(clientAudiences.Select(x => x.Audience)),
            IssuedAt = token.IssuedAt.ToUnixTimeSeconds(),
            NotBefore = token.NotBefore?.ToUnixTimeSeconds(),
            Expiration = token.ExpiredAt.ToUnixTimeSeconds(),
            Scope = string.Join(" ", token.Scopes.Select(x => x.ClientScope)),
            Subject = token.Subject
        };
        
        if (token.SessionId.HasValue)
        {
            var session = await _sessionQueryService.GetByIdAsync(token.SessionId.Value, cancellationToken);
            if (session is null) return Results.Success(SuccessCodes.Ok, IntrospectionResponse.Fail());
            
            var user = await _userQueryService.GetByIdAsync(session.UserId, cancellationToken);
            if (user is null) return Results.Success(SuccessCodes.Ok, IntrospectionResponse.Fail());

            response.Subject = user.Id.ToString();
            response.Username = user.Username;
        }

        return Results.Success(SuccessCodes.Ok, response);
    }
}

public sealed class IntrospectionCommandValidator : IRequestValidator<IntrospectionCommand>
{
    public async ValueTask<Result> Validate(IntrospectionCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'token' is required"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }
}