namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Extraction;

public abstract class ExtractionResult
{
    public bool IsSucceeded { get; protected init; }
}

public abstract class ExtractionResult<TValue> : ExtractionResult
{
    public TValue? Value { get; private init; }

    protected static TDerived Success<TDerived>(TValue value) where TDerived : ExtractionResult<TValue>, new()
    {
        return new TDerived()
        {
            IsSucceeded =  true,
            Value = value
        };
    }

    protected static TDerived Fail<TDerived>() where TDerived : ExtractionResult<TValue>, new()
    {
        return new TDerived()
        {
            IsSucceeded =  false,
        };
    }
}