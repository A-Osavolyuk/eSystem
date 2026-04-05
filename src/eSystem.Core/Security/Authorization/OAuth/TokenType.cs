using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueConverter<TokenType>))]
public enum TokenType
{
    [EnumValue("urn:ietf:params:oauth:token-type:access_token", true)]
    [EnumValue("access_token")]
    AccessToken,
    
    [EnumValue("urn:ietf:params:oauth:token-type:refresh_token", true)]
    [EnumValue("refresh_token")]
    RefreshToken,
    
    [EnumValue("urn:ietf:params:oauth:token-type:id_token", true)]
    [EnumValue("id_token")]
    IdToken,
    
    [EnumValue("urn:ietf:params:oauth:token-type:logout_token" ,true)]
    [EnumValue("logout_token")]
    LogoutToken,
    
    [EnumValue("login_token")]
    LoginToken,
    
    [EnumValue("urn:ietf:params:oauth:token-type:jwt", true)]
    [EnumValue("jwt")]
    Jwt,
}