using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authentication.Oidc.Token;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Cryptography.Tokens;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Security.Authentication.Oidc.Introspection;
using eSystem.Core.Security.Authentication.Oidc.Revocation;

namespace eSecurity.Server.Features.Connect.Commands;

public record IntrospectionCommand(IntrospectionRequest Request) : IRequest<Result>;

public class IntrospectionCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    IHasherFactory hasherFactory,
    IOptions<TokenOptions> options) : IRequestHandler<IntrospectionCommand, Result>
{
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IHasherFactory _hasherFactory = hasherFactory;
    private readonly TokenOptions _options = options.Value;

    public async Task<Result> Handle(IntrospectionCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Request.Token))
            return Results.BadRequest(new Error()
            {
                Code = Errors.OAuth.InvalidRequest,
                Description = "token is required"
            });

        OpaqueTokenType? opaqueTokenType = request.Request.TokenTypeHint switch
        {
            TokenTypeHints.AccessToken => OpaqueTokenType.AccessToken,
            TokenTypeHints.RefreshToken => OpaqueTokenType.RefreshToken,
            _ => null
        };
        
        var hasher = _hasherFactory.CreateHasher(HashAlgorithm.Sha512);
        var incomingHash = hasher.Hash(request.Request.Token);

        var token = opaqueTokenType.HasValue
            ? await _tokenManager.FindByTokenAsync(incomingHash, opaqueTokenType.Value, cancellationToken)
            : await _tokenManager.FindByTokenAsync(incomingHash, cancellationToken);
        
        if (token is null || !token.IsValid) return Results.Ok(IntrospectionResponse.Fail());
        
        var user = await _userManager.FindByIdAsync(token.Session.Device.UserId, cancellationToken);
        if (user is null) return Results.Ok(IntrospectionResponse.Fail());
        
        var tokenType = token.TokenType switch
        {
            OpaqueTokenType.AccessToken => IntrospectionTokenTypes.AccessToken,
            OpaqueTokenType.RefreshToken => IntrospectionTokenTypes.RefreshToken,
            _ => throw new NotSupportedException("Unsupported token type")
        };
        
        var response = new IntrospectionResponse()
        {
            Active = true,
            TokenType = tokenType,
            ClientId = token.Client.Id,
            Issuer = _options.Issuer,
            Audience = token.Client.Audience,
            Subject = token.Session.Device.UserId.ToString(),
            Username = user.Username,
            IssuedAt = token.CreateDate!.Value.ToUnixTimeSeconds(),
            Expiration = token.ExpiredDate.ToUnixTimeSeconds(),
            Scope = string.Join(" ", token.Scopes.Select(x => x.Scope.Name))
        };
        
        return Results.Ok(response);
    }
}