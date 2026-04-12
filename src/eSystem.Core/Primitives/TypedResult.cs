namespace eSystem.Core.Primitives;

public sealed class TypedResult<TValue>
{
    public bool Succeeded { get; init; }
    private TValue? Value { get; init; }
    private Error? Error { get; init; }

    private TypedResult() {}

    public static TypedResult<TValue> Success(TValue value) => new() { Value = value, Succeeded = true };
    public static TypedResult<TValue> Fail(Error error) => new() { Error = error, Succeeded = false };

    public Error GetError() => Error!;

    public bool TryGetValue(out TValue value)
    {
        if (!Succeeded || Value is null)
        {
            value = default!;
            return false;
        }

        value = Value;
        return true;
    }
}