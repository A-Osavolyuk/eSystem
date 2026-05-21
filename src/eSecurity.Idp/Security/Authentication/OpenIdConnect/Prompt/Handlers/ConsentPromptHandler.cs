using eSecurity.Idp.Security.Authentication.OpenIdConnect.Client;
using eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Idp.Security.Authorization.OAuth.Consents;
using eSecurity.Idp.Security.Authorization.OAuth.Scopes;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt.Handlers;

public sealed class ConsentPromptHandler(
    ISessionManager sessionManager,
    IClientManager clientManager,
    IConsentManager consentManager,
    IUserManager userManager,
    IOptions<OpenIdConfiguration> options,
    ISessionAccessor sessionAccessor,
    IPromptStateFactory stateFactory) : IPromptHandler
{
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IClientManager _clientManager = clientManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly IPromptStateFactory _stateFactory = stateFactory;
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

        context.Prompts.Remove(PromptType.Consent);
        if (context.Prompts.Count == 0)
            context.Prompts.Add(PromptType.None);

        try
        {
            var state = _stateFactory.CreateState(context);
            var url = QueryBuilder.Create()
                .WithUri("https://localhost:6521/connect/consents")
                .WithQueryParam("state", state)
                .Build();

            return PromptResult.Success(
                Results.Redirect(RedirectionCode.Found, url)
            );
        }
        catch (Exception)
        {
            var uri = QueryBuilder.Create()
                .WithUri(context.RedirectUri)
                .WithQueryParam("error", ErrorCode.ServerError)
                .WithQueryParam("error_description", "Server error")
                .Build();
            
            return PromptResult.Success(
                Results.Redirect(RedirectionCode.Found, uri)
            );
        }
    }
}