using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueConverter<JwtTokenType>))]
public enum JwtTokenType
{
    [EnumValue("at+jwt")]
    AccessToken,
    
    [EnumValue("id-jwt")]
    IdToken,
    
    [EnumValue("JWT")]
    Generic
}