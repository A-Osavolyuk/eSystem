using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Idp.Security.Authorization.Authorize;
using eSecurity.Idp.Security.Authorization.OAuth.Consents;
using eSecurity.Idp.Security.Authorization.OAuth.Protocol;
using eSecurity.Idp.Security.Authorization.OAuth.Scopes;
using eSecurity.Idp.Security.Authorization.OAuth.Token.AuthorizationCode;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt.Handlers;

public sealed class NonePromptHandler(
    ISessionManager sessionManager,
    IClientManager clientManager,
    IUserManager userManager,
    IConsentManager consentManager,
    IAuthorizationCodeManager authorizationCodeManager,
    ISessionAccessor sessionAccessor,
    RedirectManager redirectManager) : IPromptHandler
{
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly RedirectManager _redirectManager = redirectManager;

    public bool CanHandle(PromptType promptType) => promptType == PromptType.None;

    public async ValueTask<PromptResult> HandleAsync(PromptContext context, CancellationToken cancellationToken)
    {
        var sessionCookie = _sessionAccessor.GetCookie();
        if (sessionCookie is null)
        {
            var uri = _redirectManager.GetRedirectUri(context.RedirectUri, ErrorCode.LoginRequired, 
                "login is required", context.State);
            
            return PromptResult.Failed(Results.Redirect(RedirectionCode.Found, uri));
        }
        
        var session = await _sessionManager.FindByIdAsync(sessionCookie.SessionId, cancellationToken);
        if (session is null)
        {
            var uri = _redirectManager.GetRedirectUri(context.RedirectUri, ErrorCode.LoginRequired, 
                "login is required", context.State);
            
            return PromptResult.Failed(Results.Redirect(RedirectionCode.Found, uri));
        }

        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
        {
            var uri = _redirectManager.GetRedirectUri(context.RedirectUri, ErrorCode.LoginRequired, 
                "login is required", context.State);
            
            return PromptResult.Failed(Results.Redirect(RedirectionCode.Found, uri));
        }

        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            var uri = _redirectManager.GetRedirectUri(context.RedirectUri, ErrorCode.LoginRequired, 
                "login is required", context.State);
            
            return PromptResult.Failed(Results.Redirect(RedirectionCode.Found, uri));
        }

        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null)
        {
            var uri = _redirectManager.GetRedirectUri(context.RedirectUri, ErrorCode.AccessDenied, 
                "The user denied consents request", context.State);
            
            return PromptResult.Failed(Results.Redirect(RedirectionCode.Found, uri));
        }

        var grantedScopes = consent.GrantedScopes
            .Select(x => x.ClientScope.Scope.Value);

        if (!ScopesValidator.Validate(grantedScopes, context.Scopes, out _))
        {
            var uri = _redirectManager.GetRedirectUri(context.RedirectUri, ErrorCode.AccessDenied, 
                "The user denied consents request", context.State);
            
            return PromptResult.Failed(Results.Redirect(RedirectionCode.Found, uri));
        }

        var protocol = context.Scopes.Contains(ScopeTypes.OpenId)
            ? AuthorizationProtocol.OpenIdConnect
            : AuthorizationProtocol.OAuth;

        var code = _authorizationCodeManager.Generate();
        var authorizationCode = new AuthorizationCodeEntity
        {
            Id = Guid.CreateVersion7(),
            ClientId = client.Id,
            UserId = user.Id,
            Protocol = protocol,
            Code = code,
            Nonce = context.Nonce,
            CodeChallenge = context.CodeChallenge,
            CodeChallengeMethod = context.CodeChallengeMethod,
            RedirectUri = context.RedirectUri,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        var codeResult = await _authorizationCodeManager.CreateAsync(authorizationCode, cancellationToken);
        if (!codeResult.Succeeded)
        {
            var error = codeResult.GetError();
            var uri = _redirectManager.GetRedirectUri(context.RedirectUri, 
                error.Code, error.Description, context.State);
            
            return PromptResult.Failed(Results.Redirect(RedirectionCode.Found, uri));
        }

        var builder = QueryBuilder.Create()
            .WithUri(context.RedirectUri)
            .WithQueryParam("code", code);

        if (!string.IsNullOrEmpty(context.State))
            builder.WithQueryParam("state", context.State);

        return PromptResult.Success(
            Results.Redirect(RedirectionCode.Found, builder.Build())
        );
    }
}