using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt;

public interface IPromptsProcessor
{
    public ValueTask<Result> ProcessAsync(PromptContext context, CancellationToken cancellationToken = default);
}