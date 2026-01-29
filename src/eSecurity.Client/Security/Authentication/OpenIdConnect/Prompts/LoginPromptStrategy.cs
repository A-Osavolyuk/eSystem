using eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;
using eSecurity.Client.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Core.Common.Routing;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;

public sealed class LoginPromptStrategy(ISessionAccessor sessionAccessor) : IPromptStrategy
{
    private readonly ISessionAccessor _sessionAccessor = sessionAccessor;

    public bool CanHandle(string? prompt) => !string.IsNullOrEmpty(prompt) && prompt == PromptTypes.Login;

    public Task<AuthorizationResult> ExecuteAsync(AuthorizationContext context)
    {
        var session = _sessionAccessor.Get();
        if (session is null)
        {
            var redirectUri = PromptUtils.GetRedirectUri(Links.Common.LoginIdentifier, context);
            return Task.FromResult(AuthorizationResult.Redirect(redirectUri));
        }

        var currentPrompt = context.Prompts.First();
        context.Prompts = context.Prompts.Except([currentPrompt]).ToList();
                
        return Task.FromResult(AuthorizationResult.Next());
    }
}