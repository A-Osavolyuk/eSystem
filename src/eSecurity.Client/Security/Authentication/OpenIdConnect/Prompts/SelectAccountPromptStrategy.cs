using eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;

public sealed class SelectAccountPromptStrategy : IPromptStrategy
{
    public bool CanHandle(string? prompt) => !string.IsNullOrEmpty(prompt) && PromptTypes.SelectAccount == prompt;

    public Task<AuthorizationResult> ExecuteAsync(AuthorizationContext context)
    {
        //TODO: Implement select_account prompt handling
        throw new NotImplementedException();
    }
}