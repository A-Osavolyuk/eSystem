using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authorization.Verification;

[JsonConverter(typeof(JsonEnumValueStringConverter<VerificationMethod>))]
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