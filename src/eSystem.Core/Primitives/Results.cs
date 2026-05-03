using System.Net;
using eSystem.Core.Primitives.Enums;

namespace eSystem.Core.Primitives;

public static class Results
{
    public static Result Html(HttpStatusCode code, string html)
    {
        return new HtmlResult
        {
            Succeeded = true,
            StatusCode = code,
            Html = html
        };
    }
    
    public static Result Information(InformationalCodes code)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return new Result
        {
            Succeeded = true,
            StatusCode = httpStatusCode
        };
    }

    public static Result Success(SuccessCodes code, object? response = null)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return new ValueResult
        {
            Succeeded = true,
            StatusCode = httpStatusCode,
            Value = response
        };
    }

    public static Result Redirect(RedirectionCode code, string uri)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return new RedirectResult
        {
            Succeeded = true,
            StatusCode = httpStatusCode,
            Uri = uri
        };
    }

    public static Result ClientError(ClientErrorCode code, Error? error = null)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return new Result
        {
            Succeeded = false,
            StatusCode = httpStatusCode,
            Error = error
        };
    }

    public static Result ServerError(ServerErrorCode code, Error? error = null)
    {
        var httpStatusCode = StatusCodeMapper.Map(code);
        return new Result
        {
            Succeeded = false,
            StatusCode = httpStatusCode,
            Error = error
        };
    }
}