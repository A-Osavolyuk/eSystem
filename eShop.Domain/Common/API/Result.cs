namespace eShop.Domain.Common.API;

public class Result
{
    private Result(){}
    
    public bool Succeeded { get; set; }
    public object? Value { get; set; }
    public string Message { get; set; } = string.Empty;
    private Error? Error { get; set; }

    public static Result Success()
    {
        var result = new Result
        {
            Succeeded = true,
        };

        return result;
    }
    
    public static Result Success(object? response)
    {
        var result = new Result
        {
            Succeeded = true,
            Value = response,
        };

        return result;
    }

    public static Result Success(string message)
    {
        var result = new Result
        {
            Succeeded = true,
            Message = message,
        };

        return result;
    }

    public static Result Success(object? response, string message)
    {
        var result = new Result
        {
            Succeeded = true,
            Value = response,
            Message = message,
        };

        return result;
    }

    public static Result Failure(Error error, object? value = null)
    {
        var result = new Result
        {
            Succeeded = false,
            Error = error,
            Message = error.Message,
            Value = value
        };
        
        return result;
    }

    public TResponse Match<TResponse>(Func<Result, TResponse> success, Func<Error, TResponse> failure)
    {
        if (Succeeded)
        {
            return success(this);
        }
        
        return failure(Error!);
    }

    public Error GetError() => Error!;

    public TResponse Get<TResponse>() => (TResponse)Value!;
}