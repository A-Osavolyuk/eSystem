using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authentication.SignIn.Session;

[JsonConverter(typeof(JsonEnumValueConverter<SignInStep>))]
public enum SignInStep
{
    [EnumValue("password")]
    Password,
    
    [EnumValue("two_factor")]
    TwoFactor,
    
    [EnumValue("oauth")]
    OAuth,
    
    [EnumValue("passkey")]
    Passkey,
    
    [EnumValue("completed")]
    Completed
}