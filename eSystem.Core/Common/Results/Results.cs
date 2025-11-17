using System.Net;
using OpenTelemetry.Trace;

namespace eSystem.Core.Common.Results;

public static class Results
{
    public static Result Ok(object response) => Result.Success(HttpStatusCode.OK, response);

    public static Result Ok() => Result.Success(HttpStatusCode.OK);
    public static Result Created() => Result.Success(HttpStatusCode.Created);
    public static Result Found(string uri) => Result.Success(HttpStatusCode.Found);
    public static Result SeeOther() => Result.Success(HttpStatusCode.SeeOther);

    public static Result BadRequest(string details)
        => Result.Failure(HttpStatusCode.BadRequest, new Error
        {
            ErrorCode = Errors.Common.BadRequest,
            ErrorDescription = details
        });

    public static Result BadRequest(string error, string details)
        => Result.Failure(HttpStatusCode.BadRequest, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });
    
    public static Result Unauthorized(string details)
        => Result.Failure(HttpStatusCode.Unauthorized, new Error
        {
            ErrorCode = Errors.Common.Unauthorized,
            ErrorDescription = details
        });

    public static Result Unauthorized(string error, string details)
        => Result.Failure(HttpStatusCode.Unauthorized, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });
    
    public static Result Forbidden(string details)
        => Result.Failure(HttpStatusCode.Forbidden, new Error
        {
            ErrorCode = Errors.Common.Forbidden,
            ErrorDescription = details
        });

    public static Result Forbidden(string error, string details)
        => Result.Failure(HttpStatusCode.Forbidden, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });
    
    public static Result NotFound(string details)
        => Result.Failure(HttpStatusCode.NotFound, new Error
        {
            ErrorCode = Errors.Common.NotFound,
            ErrorDescription = details
        });

    public static Result NotFound(string error, string details)
        => Result.Failure(HttpStatusCode.NotFound, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });
    
    public static Result TooManyRequests(string details)
        => Result.Failure(HttpStatusCode.TooManyRequests, new Error
        {
            ErrorCode = Errors.Common.TooManyRequests,
            ErrorDescription = details
        });

    public static Result TooManyRequests(string error, string details)
        => Result.Failure(HttpStatusCode.TooManyRequests, new Error
        {
            ErrorCode = error,
            ErrorDescription = details
        });

    public static Result InternalServerError(string details)
        => Result.Failure(HttpStatusCode.InternalServerError, new Error
        {
            ErrorCode = Errors.Common.InternalServerError,
            ErrorDescription = details
        });

    public static Result InternalServerError(string code, string details)
        => Result.Failure(HttpStatusCode.InternalServerError, new Error
        {
            ErrorCode = code,
            ErrorDescription = details
        });
}