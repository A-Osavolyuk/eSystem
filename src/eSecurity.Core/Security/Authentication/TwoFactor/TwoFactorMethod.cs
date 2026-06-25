using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Authentication.TwoFactor;

[JsonConverter(typeof(JsonEnumValueConverter<TwoFactorMethod>))]
public enum TwoFactorMethod
{
    [JsonPropertyName("none")]
    None,
    
    [EnumValue("authenticator_app")]
    AuthenticatorApp,
    
    [EnumValue("software_key")]
    SoftwareKey,
    
    [EnumValue("hardware_key")]
    HardwareKey,
    
    [EnumValue("recovery_code")]
    RecoveryCode,
    
    [EnumValue("sms_otp")]
    SmsOtp,
    
    [EnumValue("email_otp")]
    EmailOtp
}