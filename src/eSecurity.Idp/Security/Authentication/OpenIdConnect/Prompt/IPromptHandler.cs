using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt;

public interface IPromptHandler
{
    public bool CanHandle(PromptType promptType);
    public ValueTask<PromptResult> HandleAsync(PromptContext context, CancellationToken cancellationToken);
}