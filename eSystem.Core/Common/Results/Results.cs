namespace eSystem.Core.Common.Results;

public static class Results
{
    public static Result NotFound(string details, object? value = null)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.NotFound,
            Message = "Not found",
            Details = details
        }, value);
    }
    
    public static Result NotFound(object? value = null)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.NotFound,
            Message = "Not found",
        }, value);
    }
    
    public static Result BadRequest(string details, object? value = null)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.BadRequest,
            Message = "Bad request",
            Details = details
        }, value);
    }
    
    public static Result BadRequest(object? value = null)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.BadRequest,
            Message = "Bad request",
        }, value);
    }
    
    public static Result InternalServerError(string details, object? value = null)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.InternalServerError,
            Message = "Internal server error",
            Details = details
        }, value);
    }
    
    public static Result InternalServerError(object? value = null)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.InternalServerError,
            Message = "Internal server error",
        }, value);
    }

    public static Result Unauthorized(string details, object? value = null)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.Unauthorized,
            Message = "Unauthorized",
            Details = details
        }, value);
    }
    
    public static Result Unauthorized(object? value = null)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.Unauthorized,
            Message = "Unauthorized",
        }, value);
    }

    public static Result Redirect(string url)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.Found,
        }, url);
    }
}