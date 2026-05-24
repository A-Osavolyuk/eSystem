using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Idp.Security.Authorization.Prompt.Handlers;

public sealed class SelectAccountPromptHandler : IPromptHandler
{
    public bool CanHandle(PromptType promptType) => promptType == PromptType.SelectAccount;
    public ValueTask<PromptResult> HandleAsync(PromptContext context, CancellationToken cancellationToken)
    {
        //TODO: Implement select_account prompt handling
        throw new NotImplementedException();
    }
}