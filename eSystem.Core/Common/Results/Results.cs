namespace eSystem.Core.Common.Results;

public static class Results
{
    public static Result Ok(object response) => Result.Success(StatusCode.Ok, response);

    public static Result Ok() => Result.Success(StatusCode.Ok);
    public static Result Created() => Result.Success(StatusCode.Created);
    public static Result Found(string uri) => Result.Success(StatusCode.Found);
    public static Result SeeOther() => Result.Success(StatusCode.SeeOther);

    public static Result BadRequest(string details)
        => Result.Failure(StatusCode.BadRequest, new Error
        {
            ErrorCode = Errors.Common.BadRequest,
            ErrorDescription = details
        });

    public static Result BadRequest(string error, string details)
        => Result.Failure(StatusCode.BadRequest, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });
    
    public static Result Unauthorized(string details)
        => Result.Failure(StatusCode.Unauthorized, new Error
        {
            ErrorCode = Errors.Common.Unauthorized,
            ErrorDescription = details
        });

    public static Result Unauthorized(string error, string details)
        => Result.Failure(StatusCode.Unauthorized, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });
    
    public static Result Forbidden(string details)
        => Result.Failure(StatusCode.Forbidden, new Error
        {
            ErrorCode = Errors.Common.Forbidden,
            ErrorDescription = details
        });

    public static Result Forbidden(string error, string details)
        => Result.Failure(StatusCode.Forbidden, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });
    
    public static Result NotFound(string details)
        => Result.Failure(StatusCode.NotFound, new Error
        {
            ErrorCode = Errors.Common.NotFound,
            ErrorDescription = details
        });

    public static Result NotFound(string error, string details)
        => Result.Failure(StatusCode.NotFound, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });
    
    public static Result TooManyRequests(string details)
        => Result.Failure(StatusCode.TooManyRequests, new Error
        {
            ErrorCode = Errors.Common.TooManyRequests,
            ErrorDescription = details
        });

    public static Result TooManyRequests(string error, string details)
        => Result.Failure(StatusCode.TooManyRequests, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });

    public static Result InternalServerError(string details)
        => Result.Failure(StatusCode.InternalServerError, new Error
        {
            ErrorCode = Errors.Common.InternalServerError,
            ErrorDescription = details
        });

    public static Result InternalServerError(string code, string details)
        => Result.Failure(StatusCode.InternalServerError, new Error
        {
            ErrorCode = code,
            ErrorDescription = details
        });
}