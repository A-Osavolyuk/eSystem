using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Primitives.Enums;

[JsonConverter(typeof(JsonEnumValueConverter<ClientErrorCode>))]
public enum ClientErrorCode
{
    [EnumValue("bad_request")]
    BadRequest = 400,
    
    [EnumValue("unauthorized")]
    Unauthorized = 401,
    
    [EnumValue("payment_required")]
    PaymentRequired = 402,
    
    [EnumValue("forbidden")]
    Forbidden = 403,
    
    [EnumValue("not_found")]
    NotFound = 404,
    
    [EnumValue("method_not_allowed")]
    MethodNotAllowed = 405,
    
    [EnumValue("not_acceptable")]
    NotAcceptable = 406,
    
    [EnumValue("proxy_authentication_required")]
    ProxyAuthenticationRequired = 407,
    
    [EnumValue("request_timeout")]
    RequestTimeout = 408,
    
    [EnumValue("conflict")]
    Conflict = 409,
    
    [EnumValue("gone")]
    Gone = 410,
    
    [EnumValue("length_required")]
    LengthRequired = 411,
    
    [EnumValue("precondition_failed")]
    PreconditionFailed = 412,
    
    [EnumValue("payload_too_large")]
    PayloadTooLarge = 413,
    
    [EnumValue("uri_too_long")]
    UriTooLong = 414,
    
    [EnumValue("unsupported_media_type")]
    UnsupportedMediaType = 415,
    
    [EnumValue("range_not_satisfiable")]
    RangeNotSatisfiable = 416,
    
    [EnumValue("expectation_failed")]
    ExpectationFailed = 417,
    
    [EnumValue("im_a_teapot")]
    ImATeapot = 418,
    
    [EnumValue("misdirected_request")]
    MisdirectedRequest = 421,
    
    [EnumValue("unprocessable_content")]
    UnprocessableContent = 422,
    
    [EnumValue("locked")]
    Locked = 423,
    
    [EnumValue("failed_dependency")]
    FailedDependency = 424,
    
    [EnumValue("too_early")]
    TooEarly = 425,
    
    [EnumValue("upgrade_required")]
    UpgradeRequired = 426,
    
    [EnumValue("precondition_required")]
    PreconditionRequired = 428,
    
    [EnumValue("too_many_requests")]
    TooManyRequests = 429,
    
    [EnumValue("request_header_fields_too_large")]
    RequestHeaderFieldsTooLarge = 431,
    
    [EnumValue("unavailable_for_legal_reasons")]
    UnavailableForLegalReasons = 451
}