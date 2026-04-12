using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Authorization.Verification;

[JsonConverter(typeof(JsonEnumValueConverter<VerificationMethod>))]
public enum VerificationMethod
{
    [EnumValue("email_otp")]
    EmailOtp,
    
    [EnumValue("sms_otp")]
    SmsOtp,
    
    [EnumValue("passkey")]
    Passkey,
    
    [EnumValue("authenticator_app")]
    AuthenticatorApp,
}