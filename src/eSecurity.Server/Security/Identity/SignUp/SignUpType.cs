using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Server.Security.Identity.SignUp;

[JsonConverter(typeof(JsonEnumValueConverter<SignUpType>))]
public enum SignUpType
{
    [EnumValue("manual")]
    Manual,
    
    [EnumValue("oauth")]
    OAuth
}