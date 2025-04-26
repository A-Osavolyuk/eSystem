namespace eShop.Domain.Common.API;

public static class Results
{
    public static Result NotFound(string details)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.NotFound,
            Message = "Not found",
            Details = details
        });
    }
    
    public static Result BadRequest(string details)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.BadRequest,
            Message = "Bad request",
            Details = details
        });
    }
    
    public static Result InternalServerError(string details)
    {
        return Result.Failure(new Error()
        {
            Code = ErrorCode.InternalServerError,
            Message = "Internal server error",
            Details = details
        });
    }
}