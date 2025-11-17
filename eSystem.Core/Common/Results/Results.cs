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
            Code = Errors.Common.BadRequest,
            Description = details
        });

    public static Result BadRequest(string error, string details)
        => Result.Failure(HttpStatusCode.BadRequest, new Error
        {
            Code = error,
            Description = details
        });
    
    public static Result Unauthorized(string details)
        => Result.Failure(HttpStatusCode.Unauthorized, new Error
        {
            Code = Errors.Common.Unauthorized,
            Description = details
        });

    public static Result Unauthorized(string error, string details)
        => Result.Failure(HttpStatusCode.Unauthorized, new Error
        {
            Code = error,
            Description = details
        });
    
    public static Result Forbidden(string details)
        => Result.Failure(HttpStatusCode.Forbidden, new Error
        {
            Code = Errors.Common.Forbidden,
            Description = details
        });

    public static Result Forbidden(string error, string details)
        => Result.Failure(HttpStatusCode.Forbidden, new Error
        {
            Code = error,
            Description = details
        });
    
    public static Result NotFound(string details)
        => Result.Failure(HttpStatusCode.NotFound, new Error
        {
            Code = Errors.Common.NotFound,
            Description = details
        });

    public static Result NotFound(string error, string details)
        => Result.Failure(HttpStatusCode.NotFound, new Error
        {
            Code = error,
            Description = details
        });
    
    public static Result TooManyRequests(string details)
        => Result.Failure(HttpStatusCode.TooManyRequests, new Error
        {
            Code = Errors.Common.TooManyRequests,
            Description = details
        });

    public static Result TooManyRequests(string error, string details)
        => Result.Failure(HttpStatusCode.TooManyRequests, new Error
        {
            Code = error,
            Description = details
        });

    public static Result InternalServerError(string details)
        => Result.Failure(HttpStatusCode.InternalServerError, new Error
        {
            Code = Errors.Common.InternalServerError,
            Description = details
        });

    public static Result InternalServerError(string code, string details)
        => Result.Failure(HttpStatusCode.InternalServerError, new Error
        {
            Code = code,
            Description = details
        });
}