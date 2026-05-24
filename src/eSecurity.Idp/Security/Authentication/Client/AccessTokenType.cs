using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Authentication.Client;

[JsonConverter(typeof(JsonEnumValueConverter<AccessTokenType>))]
public enum AccessTokenType
{
    [EnumValue("jwt")]
    Jwt,
    
    [EnumValue("reference")]
    Reference
}