using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Authorization;

[JsonConverter(typeof(JsonEnumValueConverter<AuthorizationProtocol>))]
public enum AuthorizationProtocol
{
    [EnumValue("openid_connect")]
    OpenIdConnect,
    
    [EnumValue("oauth")]
    OAuth
}