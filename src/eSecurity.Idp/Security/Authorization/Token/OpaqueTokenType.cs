using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Authorization.Token;

[JsonConverter(typeof(JsonEnumValueConverter<OpaqueTokenType>))]
public enum OpaqueTokenType
{
    [EnumValue("access_token")]
    AccessToken,
    
    [EnumValue("refresh_token")]
    RefreshToken,
    
    [EnumValue("login_token")]
    LoginToken
}