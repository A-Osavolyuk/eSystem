using System.Text.Json;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.OAuth.Consents;
using eSecurity.Server.Security.Authorization.OAuth.Protocol;
using eSecurity.Server.Security.Authorization.OAuth.Scopes;
using eSecurity.Server.Security.Authorization.OAuth.Token.AuthorizationCode;
using eSecurity.Server.Security.Cookies;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Utilities.Query;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Prompt.Handlers;

public sealed class NonePromptHandler(
    ISessionManager sessionManager,
    IClientManager clientManager,
    IUserManager userManager,
    IConsentManager consentManager,
    IAuthorizationCodeManager authorizationCodeManager,
    ISessionAccessor sessionAccessor) : IPromptHandler
{
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly IAuthorizationCodeManager _authorizationCodeManager = authorizationCodeManager;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;

    public bool CanHandle(PromptType promptType) => promptType == PromptType.None;

    public async ValueTask<PromptResult> HandleAsync(PromptContext context, CancellationToken cancellationToken)
    {
        var sessionCookie = _sessionAccessor.GetCookie();
        if (sessionCookie is null)
        {
            return PromptResult.Failed(
                Results.Redirect(RedirectionCode.Found, PromptHelper.GetRedirectUri(context.RedirectUri, 
                    ErrorCode.LoginRequired, "Login required", context.State)
                )
            );
        }
        
        var session = await _sessionManager.FindByIdAsync(sessionCookie.SessionId, cancellationToken);
        if (session is null)
        {
            return PromptResult.Failed(
                Results.Redirect(RedirectionCode.Found, PromptHelper.GetRedirectUri(context.RedirectUri,
                    ErrorCode.LoginRequired, "Login required", context.State)
                )
            );
        }

        var client = await _clientManager.FindByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
        {
            return PromptResult.Failed(
                Results.Redirect(RedirectionCode.Found, PromptHelper.GetRedirectUri(context.RedirectUri,
                    ErrorCode.LoginRequired, "Login required", context.State)
                )
            );
        }

        var user = await _userManager.FindByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            return PromptResult.Failed(
                Results.Redirect(RedirectionCode.Found, PromptHelper.GetRedirectUri(context.RedirectUri,
                    ErrorCode.LoginRequired, "Login required", context.State)
                )
            );
        }

        var consent = await _consentManager.FindAsync(user, client, cancellationToken);
        if (consent is null)
        {
            return PromptResult.Failed(
                Results.Redirect(RedirectionCode.Found, PromptHelper.GetRedirectUri(context.RedirectUri,
                    ErrorCode.AccessDenied, "The user denied consents request", context.State)
                )
            );
        }

        var grantedScopes = consent.GrantedScopes
            .Select(x => x.ClientScope.Scope.Value);

        if (!ScopesValidator.Validate(grantedScopes, context.Scopes, out _))
        {
            return PromptResult.Failed(
                Results.Redirect(RedirectionCode.Found, PromptHelper.GetRedirectUri(context.RedirectUri,
                    ErrorCode.AccessDenied, "The user denied consents request", context.State)
                )
            );
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
            return PromptResult.Failed(
                Results.Redirect(RedirectionCode.Found, PromptHelper.GetRedirectUri(context.RedirectUri,
                    error.Code, error.Description, context.State)
                )
            );
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