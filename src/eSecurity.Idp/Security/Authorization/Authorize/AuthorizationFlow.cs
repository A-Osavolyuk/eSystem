using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Idp.Security.Authorization.Authorize;

[JsonConverter(typeof(JsonNumberEnumConverter<AuthorizationFlow>))]
public enum AuthorizationFlow
{
    [EnumValue("manual")]
    Manual,
    
    [EnumValue("par")]
    PushedAuthorizationRequest,
    
    [EnumValue("jar")]
    JwtSecuredAuthorizationRequest
}