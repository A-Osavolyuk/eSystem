using System.Text.Json;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.OAuth.Token;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Binding;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Introspection;

namespace eSecurity.Server.Features.Connect.Commands;

public record IntrospectionCommand(IFormCollection Form) : IRequest<Result>;

public class IntrospectionCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    IHasherProvider hasherProvider,
    ISessionManager sessionManager,
    IOptions<TokenConfigurations> options,
    IFormBindingProvider bindingProvider) : IRequestHandler<IntrospectionCommand, Result>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IFormBindingProvider _bindingProvider = bindingProvider;
    private readonly TokenConfigurations _configurations = options.Value;

    public async Task<Result> Handle(IntrospectionCommand request, CancellationToken cancellationToken)
    {
        var binder = _bindingProvider.GetRequiredBinder<IntrospectionRequest>();
        var bindingResult = await binder.BindAsync(request.Form, cancellationToken);
        if (!bindingResult.Succeeded || !bindingResult.TryGetValue(out var introspectionRequest))
        {
            var error = bindingResult.GetError();
            return Results.ClientError(ClientErrorCode.BadRequest, error);
        }

        OpaqueTokenType? opaqueTokenType = introspectionRequest.TokenTypeHint switch
        {
            TokenTypeHint.AccessToken => OpaqueTokenType.AccessToken,
            TokenTypeHint.RefreshToken => OpaqueTokenType.RefreshToken,
            _ => null
        };

        var hasher = _hasherProvider.GetHasher(HashAlgorithm.Sha512);
        var incomingHash = hasher.Hash(introspectionRequest.Token);

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

        var response = new IntrospectionResponse
        {
            Active = true,
            TokenType = tokenType,
            ClientId = token.Client.Id,
            Issuer = _configurations.Issuer,
            Audience = JsonSerializer.Serialize(token.Client.Audiences),
            IssuedAt = token.IssuedAt.ToUnixTimeSeconds(),
            NotBefore = token.NotBefore?.ToUnixTimeSeconds(),
            Expiration = token.ExpiredAt.ToUnixTimeSeconds(),
            Scope = string.Join(" ", token.Scopes.Select(x => x.ClientScope)),
            Subject = token.Subject
        };
        
        if (token.SessionId.HasValue)
        {
            var session = await _sessionManager.FindByIdAsync(token.SessionId.Value, cancellationToken);
            if (session is null) return Results.Success(SuccessCodes.Ok, IntrospectionResponse.Fail());
            
            var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
            if (user is null) return Results.Success(SuccessCodes.Ok, IntrospectionResponse.Fail());

            response.Subject = user.Id.ToString();
            response.Username = user.Username;
        }

        return Results.Success(SuccessCodes.Ok, response);
    }
}