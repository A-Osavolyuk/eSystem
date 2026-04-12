using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<AuthenticationMethodReference>))]
public enum AuthenticationMethodReference
{
    [EnumValue("pwd")]
    Password,
    
    [EnumValue("otp")]
    OneTimePassword,
    
    [EnumValue("swk")]
    SoftwareKey,
    
    [EnumValue("hwk")]
    HardwareKey,
    
    [EnumValue("saml")]
    Saml,
    
    [EnumValue("sms")]
    SmsOtp,
    
    [EnumValue("kba")]
    KnowledgeBasedAuthentication,
    
    [EnumValue("fpt")]
    Fingerprint,
    
    [EnumValue("face")]
    FaceRecognition,
    
    [EnumValue("eye")]
    IrisScan,
    
    [EnumValue("oauth")]
    OAuth,
    
    [EnumValue("oidc")]
    OpenIdConnect,
    
    [EnumValue("ema")]
    EmailBasedAuthentication
}