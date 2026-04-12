using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Server.Security.Authorization.OAuth.Protocol;

[JsonConverter(typeof(JsonEnumValueConverter<AuthorizationProtocol>))]
public enum AuthorizationProtocol
{
    [EnumValue("openid_connect")]
    OpenIdConnect,
    
    [EnumValue("oauth")]
    OAuth
}