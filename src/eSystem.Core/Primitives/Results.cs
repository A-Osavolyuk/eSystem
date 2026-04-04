using System.Net;

namespace eSystem.Core.Primitives;

public static class Results
{
    public static Result Ok(object response) => Result.Success(HttpStatusCode.OK, response);

    public static Result Ok() => Result.Success(HttpStatusCode.OK);
    public static Result Created(object? response = null) => Result.Success(HttpStatusCode.Created, response);
    public static Result Found(string uri) => Result.Success(HttpStatusCode.Found, uri);
    public static Result SeeOther(string uri) => Result.Success(HttpStatusCode.SeeOther, uri);

    public static Result BadRequest(string description)
        => Result.Failure(HttpStatusCode.BadRequest, new Error
        {
            Code = ErrorType.Common.BadRequest,
            Description = description
        });

    public static Result BadRequest(Error error)
        => Result.Failure(HttpStatusCode.BadRequest, error);

    public static Result Unauthorized(string description)
        => Result.Failure(HttpStatusCode.Unauthorized, new Error
        {
            Code = ErrorType.Common.Unauthorized,
            Description = description
        });

    public static Result Unauthorized(Error error)
        => Result.Failure(HttpStatusCode.Unauthorized, error);

    public static Result Forbidden(string description)
        => Result.Failure(HttpStatusCode.Forbidden, new Error
        {
            Code = ErrorType.Common.Forbidden,
            Description = description
        });

    public static Result Forbidden(Error error)
        => Result.Failure(HttpStatusCode.Forbidden, error);

    public static Result NotFound(string description)
        => Result.Failure(HttpStatusCode.NotFound, new Error
        {
            Code = ErrorType.Common.NotFound,
            Description = description
        });

    public static Result NotFound(Error error)
        => Result.Failure(HttpStatusCode.NotFound, error);

    public static Result TooManyRequests(string description)
        => Result.Failure(HttpStatusCode.TooManyRequests, new Error
        {
            Code = ErrorType.Common.TooManyRequests,
            Description = description
        });

    public static Result TooManyRequests(Error error)
        => Result.Failure(HttpStatusCode.TooManyRequests, error);

    public static Result InternalServerError(string description)
        => Result.Failure(HttpStatusCode.InternalServerError, new Error
        {
            Code = ErrorType.Common.InternalServerError,
            Description = description
        });

    public static Result InternalServerError(Error error)
        => Result.Failure(HttpStatusCode.InternalServerError, error);
}