using eSystem.Core.Primitives;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Prompt;

public sealed class PromptResult
{
    public Result? Result { get; private init; }
    public required PromptState State { get; init; }
    
    private PromptResult() {}

    public static PromptResult Failed(Result result) => new()
    {
        Result = result,
        State = PromptState.Failed
    };

    public static PromptResult Success(Result result) => new()
    {
        Result = result,
        State = PromptState.Success
    };

    public static PromptResult Next() => new()
    {
        State = PromptState.Next
    };
}