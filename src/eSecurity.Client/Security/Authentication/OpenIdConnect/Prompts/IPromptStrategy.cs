using eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;

public interface IPromptStrategy
{
    public bool CanHandle(string? prompt);
    public Task<AuthorizationResult> ExecuteAsync(AuthorizationContext context);
}