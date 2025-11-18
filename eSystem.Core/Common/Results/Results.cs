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

    public static Result BadRequest(string description)
        => Result.Failure(HttpStatusCode.BadRequest, new Error
        {
            Code = Errors.Common.BadRequest,
            Description = description
        });

    public static Result BadRequest(string error, string description)
        => Result.Failure(HttpStatusCode.BadRequest, new Error
        {
            Code = error,
            Description = description
        });
    
    public static Result BadRequest(string error, string description, Dictionary<string, object> details)
        => Result.Failure(HttpStatusCode.BadRequest, new Error
        {
            Code = error,
            Description = description,
            Details = details
        });
    
    public static Result Unauthorized(string description)
        => Result.Failure(HttpStatusCode.Unauthorized, new Error
        {
            Code = Errors.Common.Unauthorized,
            Description = description
        });

    public static Result Unauthorized(string error, string description)
        => Result.Failure(HttpStatusCode.Unauthorized, new Error
        {
            Code = error,
            Description = description
        });
    
    public static Result Forbidden(string description)
        => Result.Failure(HttpStatusCode.Forbidden, new Error
        {
            Code = Errors.Common.Forbidden,
            Description = description
        });

    public static Result Forbidden(string error, string description)
        => Result.Failure(HttpStatusCode.Forbidden, new Error
        {
            Code = error,
            Description = description
        });
    
    public static Result NotFound(string description)
        => Result.Failure(HttpStatusCode.NotFound, new Error
        {
            Code = Errors.Common.NotFound,
            Description = description
        });

    public static Result NotFound(string error, string description)
        => Result.Failure(HttpStatusCode.NotFound, new Error
        {
            Code = error,
            Description = description
        });
    
    public static Result NotFound(string error, string description, Dictionary<string, object> details)
        => Result.Failure(HttpStatusCode.NotFound, new Error
        {
            Code = error,
            Description = description,
            Details = details
        });
    
    public static Result TooManyRequests(string description)
        => Result.Failure(HttpStatusCode.TooManyRequests, new Error
        {
            Code = Errors.Common.TooManyRequests,
            Description = description
        });

    public static Result TooManyRequests(string error, string description)
        => Result.Failure(HttpStatusCode.TooManyRequests, new Error
        {
            Code = error,
            Description = description
        });

    public static Result InternalServerError(string description)
        => Result.Failure(HttpStatusCode.InternalServerError, new Error
        {
            Code = Errors.Common.InternalServerError,
            Description = description
        });

    public static Result InternalServerError(string code, string description)
        => Result.Failure(HttpStatusCode.InternalServerError, new Error
        {
            Code = code,
            Description = description
        });
}