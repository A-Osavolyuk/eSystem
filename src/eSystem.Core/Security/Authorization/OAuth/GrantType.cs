using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueStringConverter<GrantType>))]
public enum GrantType
{
    [EnumValue("implicit")]
    Implicit,
    
    [EnumValue("authorization_code")]
    AuthorizationCode,
    
    [EnumValue("refresh_token")]
    RefreshToken,
    
    [EnumValue("client_credentials")]
    ClientCredentials,
    
    [EnumValue("password")]
    Password,
    
    [EnumValue("urn:ietf:params:oauth:grant-type:device_code")]
    DeviceCode,
    
    [EnumValue("urn:ietf:params:oauth:grant-type:jwt-bearer")]
    JwtBearer,
    
    [EnumValue("urn:ietf:params:oauth:grant-type:saml2-bearer-bearer")]
    Saml2Bearer,
    
    [EnumValue("urn:ietf:params:oauth:grant-type:token-exchange")]
    TokenExchange,
    
    [EnumValue("urn:openid:params:grant-type:ciba")]
    Ciba
}