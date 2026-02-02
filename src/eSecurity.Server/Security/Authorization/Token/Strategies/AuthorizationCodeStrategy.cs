using eSecurity.Server.Security.Authorization.Token.AuthorizationCode;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Token.Strategies;

public sealed class AuthorizationCodeContext : TokenContext
{
    public string? RedirectUri { get; set; }
    public string? Code { get; set; }
    public string? CodeVerifier { get; set; }
}

public class AuthorizationCodeStrategy(
    IAuthorizationCodeManager authorizationCodeManager,
    IAuthorizationCodeFlowResolver resolver) : ITokenStrategy
{
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly IAuthorizationCodeFlowResolver _resolver = resolver;

    public async ValueTask<Result> ExecuteAsync(TokenContext context,
        CancellationToken cancellationToken = default)
    {
        if (context is not AuthorizationCodeContext authorizationContext)
            throw new NotSupportedException("Payload type must be 'AuthorizationCodeTokenPayload'");

        if (string.IsNullOrEmpty(authorizationContext.RedirectUri))
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "redirect_uri is required"
            });

        var code = authorizationContext.Code!;
        var redirectUri = authorizationContext.RedirectUri;
        var authorizationCode = await _authorizationCodeManager.FindByCodeAsync(code, cancellationToken);

        if (authorizationCode is null || authorizationCode.Used ||
            authorizationCode.ExpireDate < DateTimeOffset.UtcNow ||
            !authorizationCode.RedirectUri.Equals(redirectUri))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidGrant,
                Description = "Invalid authorization code."
            });
        }

        var flow = _resolver.Resolve(authorizationCode.Protocol);
        return await flow.ExecuteAsync(authorizationContext, authorizationCode, cancellationToken);
    }
}