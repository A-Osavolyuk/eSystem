using eSecurity.Client.Security.Authentication.OpenIdConnect.Authorization;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Prompts;

public sealed class SelectAccountPromptStrategy : IPromptStrategy
{
    public bool CanHandle(PromptType? prompt) => prompt is PromptType.SelectAccount;

    public Task<AuthorizationResult> ExecuteAsync(AuthorizationContext context)
    {
        //TODO: Implement select_account prompt handling
        throw new NotImplementedException();
    }
}