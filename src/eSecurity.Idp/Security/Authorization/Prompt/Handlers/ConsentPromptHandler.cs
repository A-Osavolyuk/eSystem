using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authentication.Session;
using eSecurity.Idp.Security.Authorization.Authorize;
using eSecurity.Idp.Security.Authorization.Consents;
using eSecurity.Idp.Security.Authorization.Scopes;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect.Discovery;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Idp.Security.Authorization.Prompt.Handlers;

public sealed class ConsentPromptHandler(
    ISessionManager sessionManager,
    IConsentManager consentManager,
    IUserQueryService userQueryService,
    IOptions<OpenIdConfiguration> options,
    ISessionAccessor sessionAccessor,
    IPromptStateFactory stateFactory,
    IClientQueryService clientQueryService,
    RedirectManager redirectManager) : IPromptHandler
{
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IConsentManager _consentManager = consentManager;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;
    private readonly IPromptStateFactory _stateFactory = stateFactory;
    private readonly IClientQueryService _clientQueryService = clientQueryService;
    private readonly RedirectManager _redirectManager = redirectManager;
    private readonly OpenIdConfiguration _configuration = options.Value;

    public bool CanHandle(PromptType promptType) => promptType == PromptType.Consent;

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

        var client = await _clientQueryService.GetByIdAsync(context.ClientId, cancellationToken);
        if (client is null)
        {
            var uri = _redirectManager.GetRedirectUri(context.RedirectUri, ErrorCode.BadRequest, 
                "invalid or unknown client_id", context.State);
            
            return PromptResult.Failed(Results.Redirect(RedirectionCode.Found, uri));
        }

        var user = await _userQueryService.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            var uri = _redirectManager.GetRedirectUri(context.RedirectUri, ErrorCode.LoginRequired, 
                "login is required", context.State);
            
            return PromptResult.Failed(Results.Redirect(RedirectionCode.Found, uri));
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
                .WithQueryParam("client_id", client.Id)
                .WithQueryParam("scope", string.Join(' ', context.Scopes))
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