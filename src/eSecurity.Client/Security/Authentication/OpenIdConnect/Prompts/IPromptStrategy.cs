using eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;

public interface IPromptStrategy
{
    public bool CanHandle(PromptType? prompt);
    public Task<AuthorizationResult> ExecuteAsync(AuthorizationContext context);
}