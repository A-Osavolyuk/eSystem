namespace eSystem.Core.Primitives;

public class ValueResult: Result
{
    public object? Value { get; init; }

    public T GetValue<T>()
    {
        if (Value is null)
            throw new NullReferenceException("Value is null");
        
        return Value is T value ? value : throw new NullReferenceException("Incorrect value type") ;
    }
}