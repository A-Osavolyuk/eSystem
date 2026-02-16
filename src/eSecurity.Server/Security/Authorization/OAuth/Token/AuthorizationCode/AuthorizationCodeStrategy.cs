using eSystem.Core.Http.Constants;
using eSystem.Core.Security.Authorization.OAuth.Token;
using eSystem.Core.Security.Authorization.OAuth.Token.AuthorizationCode;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;

public class AuthorizationCodeStrategy(
    IAuthorizationCodeManager authorizationCodeManager,
    IAuthorizationCodeFlowResolver resolver) : ITokenStrategy
{
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly IAuthorizationCodeFlowResolver _resolver = resolver;

    public async ValueTask<Result> ExecuteAsync(TokenRequest tokenRequest,
        CancellationToken cancellationToken = default)
    {
        if (tokenRequest is not AuthorizationCodeRequest request)
            throw new NotSupportedException("Payload type must be 'AuthorizationCodeRequest'");

        if (string.IsNullOrEmpty(request.RedirectUri))
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.OAuth.InvalidRequest,
                Description = "redirect_uri is required"
            });

        var code = request.Code!;
        var redirectUri = request.RedirectUri;
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
        
        var context = new AuthorizationCodeFlowContext()
        {
            ClientId = request.ClientId,
            GrantType = request.GrantType,
            RedirectUri = request.RedirectUri,
            CodeVerifier = request.CodeVerifier
        };
        var flow = _resolver.Resolve(authorizationCode.Protocol);
        return await flow.ExecuteAsync(authorizationCode, context, cancellationToken);
    }
}