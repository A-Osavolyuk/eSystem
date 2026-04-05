using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

[JsonConverter(typeof(JsonEnumValueStringConverter<OpaqueTokenType>))]
public enum OpaqueTokenType
{
    [EnumValue("access_token")]
    AccessToken,
    
    [EnumValue("refresh_token")]
    RefreshToken,
    
    [EnumValue("login_token")]
    LoginToken
}