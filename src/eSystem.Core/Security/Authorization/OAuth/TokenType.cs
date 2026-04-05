using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueStringConverter<TokenType>))]
public enum TokenType
{
    [EnumValue("access_token", true)]
    [EnumValue("urn:ietf:params:oauth:token-type:access_token")]
    AccessToken,
    
    [EnumValue("refresh_token", true)]
    [EnumValue("urn:ietf:params:oauth:token-type:refresh_token")]
    RefreshToken,
    
    [EnumValue("id_token", true)]
    [EnumValue("urn:ietf:params:oauth:token-type:id_token")]
    IdToken,
    
    [EnumValue("login_token", true)]
    [EnumValue("urn:ietf:params:oauth:token-type:login_token")]
    LoginToken,
    
    [EnumValue("logout_token")]
    LogoutToken,
    
    [EnumValue("jwt", true)]
    [EnumValue("urn:ietf:params:oauth:token-type:jwt")]
    Jwt,
}