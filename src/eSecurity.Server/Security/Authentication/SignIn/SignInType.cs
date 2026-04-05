using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authentication.SignIn;

[JsonConverter(typeof(JsonEnumValueStringConverter<SignInType>))]
public enum SignInType
{
    [EnumValue("password")]
    Password,
    
    [EnumValue("passkey")]
    Passkey,
    
    [EnumValue("oauth")]
    OAuth,
    
    [EnumValue("two_factor")]
    TwoFactor
}