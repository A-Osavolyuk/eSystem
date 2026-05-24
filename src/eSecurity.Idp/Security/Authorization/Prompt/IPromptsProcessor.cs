using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Prompt;

public interface IPromptsProcessor
{
    public ValueTask<Result> ProcessAsync(PromptContext context, CancellationToken cancellationToken = default);
}