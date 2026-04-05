using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

[JsonConverter(typeof(JsonEnumValueConverter<AccessTokenType>))]
public enum AccessTokenType
{
    [EnumValue("jwt")]
    Jwt,
    
    [EnumValue("reference")]
    Reference
}