using System.Net;
using eSystem.Core.Primitives.Enums;

namespace eSystem.Core.Primitives;

public static class StatusCodeMapper
{
    public static HttpStatusCode Map(InformationalCodes code)
    {
        return code switch
        {
            InformationalCodes.Continue => HttpStatusCode.Continue,
            InformationalCodes.SwitchingProtocols => HttpStatusCode.SwitchingProtocols,
            InformationalCodes.Processing => HttpStatusCode.Processing,
            InformationalCodes.EarlyHints => HttpStatusCode.EarlyHints,
            _ => throw new NotSupportedException($"Unsupported informational status code: {code}")
        };
    }

    public static HttpStatusCode Map(SuccessCodes code)
    {
        return code switch
        {
            SuccessCodes.Ok => HttpStatusCode.OK,
            SuccessCodes.Created => HttpStatusCode.Created,
            SuccessCodes.Accepted => HttpStatusCode.Accepted,
            SuccessCodes.NonAuthoritativeInformation => HttpStatusCode.NonAuthoritativeInformation,
            SuccessCodes.NoContent => HttpStatusCode.NoContent,
            SuccessCodes.ResetContent => HttpStatusCode.ResetContent,
            SuccessCodes.PartialContent => HttpStatusCode.PartialContent,
            SuccessCodes.MultiStatus => HttpStatusCode.MultiStatus,
            SuccessCodes.AlreadyReported => HttpStatusCode.AlreadyReported,
            SuccessCodes.ImUsed => HttpStatusCode.IMUsed,
            _ => throw new NotSupportedException($"Unsupported success status code: {code}")
        };
    }
    
    public static HttpStatusCode Map(RedirectionCode code)
    {
        return code switch
        {
            RedirectionCode.MultipleChoices => HttpStatusCode.MultipleChoices,
            RedirectionCode.MovedPermanently => HttpStatusCode.MovedPermanently,
            RedirectionCode.Found => HttpStatusCode.Found,
            RedirectionCode.SeeOther => HttpStatusCode.SeeOther,
            RedirectionCode.NotModified => HttpStatusCode.NotModified,
            RedirectionCode.UseProxy => HttpStatusCode.UseProxy,
            RedirectionCode.Unused => HttpStatusCode.Unused,
            RedirectionCode.TemporaryRedirect => HttpStatusCode.TemporaryRedirect,
            RedirectionCode.PermanentRedirect => HttpStatusCode.PermanentRedirect,
            _ => throw new NotSupportedException($"Unsupported redirection status code: {code}")
        };
    }

    public static HttpStatusCode Map(ClientErrorCode code)
    {
        return code switch
        {
            ClientErrorCode.BadRequest => HttpStatusCode.BadRequest,
            ClientErrorCode.Unauthorized => HttpStatusCode.Unauthorized,
            ClientErrorCode.PaymentRequired => HttpStatusCode.PaymentRequired,
            ClientErrorCode.Forbidden => HttpStatusCode.Forbidden,
            ClientErrorCode.NotFound => HttpStatusCode.NotFound,
            ClientErrorCode.MethodNotAllowed => HttpStatusCode.MethodNotAllowed,
            ClientErrorCode.NotAcceptable => HttpStatusCode.NotAcceptable,
            ClientErrorCode.ProxyAuthenticationRequired => HttpStatusCode.ProxyAuthenticationRequired,
            ClientErrorCode.RequestTimeout => HttpStatusCode.RequestTimeout,
            ClientErrorCode.Conflict => HttpStatusCode.Conflict,
            ClientErrorCode.Gone => HttpStatusCode.Gone,
            ClientErrorCode.LengthRequired => HttpStatusCode.LengthRequired,
            ClientErrorCode.PreconditionFailed => HttpStatusCode.PreconditionFailed,
            ClientErrorCode.PayloadTooLarge => HttpStatusCode.RequestEntityTooLarge,
            ClientErrorCode.UriTooLong => HttpStatusCode.RequestUriTooLong,
            ClientErrorCode.UnsupportedMediaType => HttpStatusCode.UnsupportedMediaType,
            ClientErrorCode.RangeNotSatisfiable => HttpStatusCode.RequestedRangeNotSatisfiable,
            ClientErrorCode.ExpectationFailed => HttpStatusCode.ExpectationFailed,
            ClientErrorCode.MisdirectedRequest => HttpStatusCode.MisdirectedRequest,
            ClientErrorCode.UnprocessableContent => HttpStatusCode.UnprocessableContent,
            ClientErrorCode.Locked => HttpStatusCode.Locked,
            ClientErrorCode.FailedDependency => HttpStatusCode.FailedDependency,
            ClientErrorCode.UpgradeRequired => HttpStatusCode.UpgradeRequired,
            ClientErrorCode.PreconditionRequired => HttpStatusCode.PreconditionRequired,
            ClientErrorCode.TooManyRequests => HttpStatusCode.TooManyRequests,
            ClientErrorCode.RequestHeaderFieldsTooLarge => HttpStatusCode.RequestHeaderFieldsTooLarge,
            ClientErrorCode.UnavailableForLegalReasons => HttpStatusCode.UnavailableForLegalReasons,
            _ => throw new NotSupportedException($"Unsupported client-side error status code: {code}")
        };
    }

    public static HttpStatusCode Map(ServerErrorCode code)
    {
        return code switch
        {
            ServerErrorCode.InternalServerError => HttpStatusCode.InternalServerError,
            ServerErrorCode.NotImplemented => HttpStatusCode.NotImplemented,
            ServerErrorCode.BadGateway => HttpStatusCode.BadGateway,
            ServerErrorCode.ServiceUnavailable => HttpStatusCode.ServiceUnavailable,
            ServerErrorCode.GatewayTimeout => HttpStatusCode.GatewayTimeout,
            ServerErrorCode.HttpVersionNotSupported => HttpStatusCode.HttpVersionNotSupported,
            ServerErrorCode.VariantAlsoNegotiate => HttpStatusCode.VariantAlsoNegotiates,
            ServerErrorCode.InsufficientStorage => HttpStatusCode.InsufficientStorage,
            ServerErrorCode.LoopDetected => HttpStatusCode.LoopDetected,
            ServerErrorCode.NotExtended => HttpStatusCode.NotExtended,
            ServerErrorCode.NetworkAuthenticationRequired => HttpStatusCode.NetworkAuthenticationRequired,
            _ => throw new NotSupportedException($"Unsupported server-side error status code: {code}")
        };
    }
}