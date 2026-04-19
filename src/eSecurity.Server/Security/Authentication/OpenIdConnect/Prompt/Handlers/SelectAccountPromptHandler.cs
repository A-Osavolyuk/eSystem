using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Prompt.Handlers;

public sealed class SelectAccountPromptHandler : IPromptHandler
{
    public bool CanHandle(PromptType promptType) => promptType == PromptType.SelectAccount;
    public ValueTask<PromptResult> HandleAsync(PromptContext context, CancellationToken cancellationToken)
    {
        //TODO: Implement select_account prompt handling
        throw new NotImplementedException();
    }
}