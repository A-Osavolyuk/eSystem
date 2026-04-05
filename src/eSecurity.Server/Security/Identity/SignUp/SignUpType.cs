using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Identity.SignUp;

[JsonConverter(typeof(JsonEnumValueStringConverter<SignUpType>))]
public enum SignUpType
{
    [EnumValue("manual")]
    Manual,
    
    [EnumValue("oauth")]
    OAuth
}