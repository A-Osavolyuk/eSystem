using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Primitives.Enums;

[JsonConverter(typeof(JsonEnumValueConverter<ServerErrorCode>))]
public enum ServerErrorCode
{
    [EnumValue("internal_server_error")]
    InternalServerError = 500,
    
    [EnumValue("not_implemented")]
    NotImplemented = 501,
    
    [EnumValue("bad_gateway")]
    BadGateway = 502,
    
    [EnumValue("service_unavailable")]
    ServiceUnavailable = 503,
    
    [EnumValue("gateway_timeout")]
    GatewayTimeout = 504,
    
    [EnumValue("http_version_not_supported")]
    HttpVersionNotSupported = 505,
    
    [EnumValue("variant_also_negotiate")]
    VariantAlsoNegotiate = 506,
    
    [EnumValue("insufficient_storage")]
    InsufficientStorage = 507,
    
    [EnumValue("loop_detected")]
    LoopDetected = 508,
    
    [EnumValue("not_extended")]
    NotExtended = 510,
    
    [EnumValue("network_authentication_required")]
    NetworkAuthenticationRequired = 511
}