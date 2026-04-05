using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Cryptography.Tokens;

[JsonConverter(typeof(JsonEnumValueStringConverter<TokenType>))]
public enum TokenType
{
    [EnumValue("access_token")]
    AccessToken,
    
    [EnumValue("refresh_token")]
    RefreshToken,
    
    [EnumValue("id_token")]
    IdToken,
    
    [EnumValue("login_token")]
    LoginToken,
    
    [EnumValue("logout_token")]
    LogoutToken
}