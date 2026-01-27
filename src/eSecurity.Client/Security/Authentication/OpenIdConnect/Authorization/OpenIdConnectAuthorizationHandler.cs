using eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;
using Microsoft.AspNetCore.Components;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;

public class OpenIdConnectAuthorizationHandler(
    IEnumerable<IPromptStrategy> strategies,
    NavigationManager navigationManager) : IOpenIdConnectAuthorizationHandler
{
    private readonly IEnumerable<IPromptStrategy> _strategies = strategies;
    private readonly NavigationManager _navigationManager = navigationManager;

    public async Task HandleAsync(AuthorizationContext context)
    {
        foreach (var strategy in _strategies)
        {
            var prompt = context.Prompts.FirstOrDefault();
            if (!strategy.CanHandle(prompt)) 
                continue;
            
            var result = await strategy.ExecuteAsync(context);
            if (!string.IsNullOrEmpty(result.RedirectUri))
            {
                _navigationManager.NavigateTo(result.RedirectUri);
            }
        }
    }
}