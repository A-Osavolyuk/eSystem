namespace eSystem.Core.Http.Results;

public sealed class TypedResult<TValue>
{
    public bool Succeeded { get; private set; }
    private TValue? Value { get; set; }
    private Error? Error { get; set; }

    private TypedResult()
    {
    }

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