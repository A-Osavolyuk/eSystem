namespace eSystem.Core.Primitives;

public sealed class RedirectResult : Result
{
    public required string Uri { get; init; }
}