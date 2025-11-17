namespace eSystem.Core.Common.Results;

public class Result
{
    private Result() {}
    
    public bool Succeeded { get; init; }
    public StatusCode StatusCode { get; init; }
    public object? Value { get; init; }
    public Error? Error { private get; init; }

    public static Result Success(StatusCode statusCode, object? value) => new()
    {
        Succeeded = true,
        StatusCode = statusCode,
        Value = value,
    };
    
    public static Result Success(StatusCode statusCode) => new()
    {
        Succeeded = true,
        StatusCode = statusCode,
    };
    
    public static Result Failure(StatusCode statusCode, Error error) => new()
    {
        Succeeded = false,
        StatusCode = statusCode,
        Error = error,
    };
    
    public static Result Failure(StatusCode statusCode, object value) => new()
    {
        Succeeded = false,
        StatusCode = statusCode,
        Value = value,
    };
    
    public Error GetError() => Error!;
    
    public TResponse Match<TResponse>(Func<Result, TResponse> success, Func<Result, TResponse> failure)
    {
        return Succeeded 
            ? success(this) 
            : failure(this);
    }
}