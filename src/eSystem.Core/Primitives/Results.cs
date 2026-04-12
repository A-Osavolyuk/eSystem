using System.Net;
using eSystem.Core.Primitives.Enums;

namespace eSystem.Core.Primitives;

public static class Results
{
    public static Result Information(InformationalCodes code)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return Result.Success(httpStatusCode);
    }

    public static Result Success(SuccessCodes code, object? response = null)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return Result.Success(httpStatusCode, response);
    }

    public static Result Redirect(RedirectionCode code, string uri)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return Result.Success(httpStatusCode, uri);
    }

    public static Result ClientError(ClientErrorCode code, Error? error = null)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return error is not null 
            ? Result.Failure(httpStatusCode, error) 
            : Result.Failure(httpStatusCode);
    }

    public static Result ServerError(ServerErrorCode code, Error? error = null)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return error is not null 
            ? Result.Failure(httpStatusCode, error) 
            : Result.Failure(httpStatusCode);
    }
}