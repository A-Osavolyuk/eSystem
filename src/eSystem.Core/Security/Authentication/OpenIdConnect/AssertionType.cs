using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<AssertionType>))]
public enum AssertionType
{
    [EnumValue("urn:ietf:params:oauth:client-assertion-type:jwt-bearer")]
    JwtBearer,
        
    [EnumValue("urn:ietf:params:oauth:client-assertion-type:saml2-bearer")]
    Saml2Bearer
}