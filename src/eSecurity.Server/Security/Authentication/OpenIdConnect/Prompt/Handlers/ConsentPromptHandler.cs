using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.OAuth.Consents;
using eSecurity.Server.Security.Authorization.OAuth.Scopes;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Enums;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Utilities.Query;
using eSystem.Core.Utilities.State;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Prompt.Handlers;

public sealed class ConsentPromptHandler(
    ISessionManager sessionManager,
    IClientManager clientManager,
    IConsentManager consentManager,
    IUserManager userManager,
    IOptions<OpenIdConfiguration> options,
    ISessionAccessor sessionAccessor) : IPromptHandler
{
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly OpenIdConfiguration _configuration = options.Value;
    
    public bool CanHandle(PromptType promptType) => promptType == PromptType.Consent;

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
                    ErrorCode.LoginRequired, "Invalid or unknown client_id", context.State)
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
        if (consent is not null)
        {
            var grantedScopes = consent.GrantedScopes
                .Select(x => x.ClientScope.Scope.Value);

            if (ScopesValidator.Validate(grantedScopes, context.Scopes, out _))
                return PromptResult.Next();
        }
        
        var prompts = context.Prompts
            .Where(x => x != PromptType.Consent)
            .Select(x => x.GetString())
            .ToList();
        
        if (prompts.Count == 0)
            prompts.Add(PromptType.None.GetString());
        
        var prompt = string.Join(' ', prompts);
        var scope = string.Join(' ', context.Scopes);
        var returnUrlBuilder = QueryBuilder.Create()
            .WithUri(_configuration.AuthorizationEndpoint)
            .WithQueryParam("response_type", context.ResponseType)
            .WithQueryParam("client_id", context.ClientId)
            .WithQueryParam("redirect_uri", context.RedirectUri)
            .WithQueryParam("scope", scope)
            .WithQueryParam("prompt", prompt);

        if (!string.IsNullOrEmpty(context.State))
            returnUrlBuilder.WithQueryParam("state", context.State);

        if (!string.IsNullOrEmpty(context.Nonce))
            returnUrlBuilder.WithQueryParam("nonce", context.Nonce);

        if (!string.IsNullOrEmpty(context.CodeChallenge) && context.CodeChallengeMethod is not null)
        {
            returnUrlBuilder.WithQueryParam("code_challenge", context.CodeChallenge);
            returnUrlBuilder.WithQueryParam("code_challenge_method", context.CodeChallengeMethod.Value);
        }

        var state = StateBuilder.Create()
            .WithData("return_url", returnUrlBuilder.Build())
            .Build();

        var url = QueryBuilder.Create()
            .WithUri("https://localhost:6501/connect/consents")
            .WithQueryParam("state", state)
            .Build();

        return PromptResult.Success(
            Results.Redirect(RedirectionCode.Found, url)
        );
    }
}