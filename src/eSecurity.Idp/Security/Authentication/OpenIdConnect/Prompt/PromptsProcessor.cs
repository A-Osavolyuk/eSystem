using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Prompt;

public sealed class PromptsProcessor(IEnumerable<IPromptHandler> handlers) : IPromptsProcessor
{
    private readonly IEnumerable<IPromptHandler> _handlers = handlers;

    public async ValueTask<Result> ProcessAsync(PromptContext context, CancellationToken cancellationToken = default)
    {
        var processingPrompt = context.Prompts.First();
        while (true)
        {
            var handler = _handlers.FirstOrDefault(h => h.CanHandle(processingPrompt));
            if (handler is null)
            {
                return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
                {
                    Code = ErrorCode.ServerError,
                    Description = $"No handler for prompt {processingPrompt}"
                });
            }

            var promptResult = await handler.HandleAsync(context, cancellationToken);
            if (promptResult.State is PromptState.Success or PromptState.Failed)
            {
                var result = promptResult.Result;
                if (result is null)
                {
                    return Results.ServerError(ServerErrorCode.InternalServerError, new Error()
                    {
                        Code = ErrorCode.ServerError,
                        Description = "Server error"
                    });
                }

                return result;
            }

            context.Prompts.Remove(processingPrompt);
            if (context.Prompts.Count == 0)
                context.Prompts.Add(PromptType.None);

            processingPrompt = context.Prompts.First();
        }
    }
}