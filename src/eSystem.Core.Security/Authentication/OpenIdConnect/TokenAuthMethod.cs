using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<TokenAuthMethod>))]
public enum TokenAuthMethod
{
    [EnumValue("none")]
    None,
    
    [EnumValue("private_key_jwt")]
    PrivateKeyJwt,
    
    [EnumValue("client_secret_jwt")]
    ClientSecretJwt,
    
    [EnumValue("client_secret_post")]
    ClientSecretPost,
    
    [EnumValue("client_secret_basic")]
    ClientSecretBasic
}