using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

[JsonConverter(typeof(JsonEnumValueConverter<AccessTokenType>))]
public enum AccessTokenType
{
    [EnumValue("jwt")]
    Jwt,
    
    [EnumValue("reference")]
    Reference
}