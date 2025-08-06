namespace eShop.Domain.Common.API;

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
    
    public static Result BadRequest(string details, object? value = null)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.BadRequest,
            Message = "Bad request",
            Details = details
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

    public static Result Redirect(string url)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.Found,
        }, url);
    }
}