using eSecurity.Client.Common.State.States;
using eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;
using eSecurity.Client.Security.Authorization.Consent;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Common.Routing;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;

public sealed class ConsentPromptStrategy(
    UserState userState,
    IConsentService consentService) : IPromptStrategy
{
    private readonly UserState _userState = userState;
    private readonly IConsentService _consentService = consentService;

    public bool CanHandle(string? prompt) => !string.IsNullOrEmpty(prompt) && prompt == PromptTypes.Consent;

    public async Task<AuthorizationResult> ExecuteAsync(AuthorizationContext context)
    {
        if (!_userState.IsAuthenticated)
        {
            var redirectUri = QueryBuilder.Create()
                .WithUri(context.RedirectUri)
                .WithQueryParam("error", ErrorTypes.OAuth.LoginRequired)
                .WithQueryParam("error_description", "User must pass authentication first.")
                .Build();
            
            return AuthorizationResult.Redirect(redirectUri);
        }

        var request = new CheckConsentRequest
        {
            ClientId = Guid.Parse(context.ClientId),
            UserId = _userState.UserId,
            Scopes = context.Scope.Split(" ").ToList()
        };

        var result = await _consentService.CheckAsync(request);
        if (result.Succeeded && result.TryGetValue<CheckConsentResponse>(out var response))
        {
            if (response is { Granted: true, RemainingScopes.Count: 0 })
            {
                var currentPrompt = context.Prompts.First();
                context.Prompts = context.Prompts.Except([currentPrompt]).ToList();
                
                return AuthorizationResult.Next();
            }

            var redirectUri = PromptUtils.GetRedirectUri(Links.Connect.Consents, context);
            return AuthorizationResult.Redirect(redirectUri);
        }
        else
        {
            var error = result.GetError();
            var redirectUri = QueryBuilder.Create()
                .WithUri(context.RedirectUri)
                .WithQueryParam("error", error.Code)
                .WithQueryParam("error_description", error.Description)
                .Build();

            return AuthorizationResult.Redirect(redirectUri);
        }
    }
}