using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueStringConverter<AuthenticationMethod>))]
public enum AuthenticationMethod
{
    [EnumValue("pwd")]
    Password,
    
    [EnumValue("mfa")]
    MultiFactorAuthentication,
    
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
    
    [EnumValue("ftp")]
    Fingerprint,
    
    [EnumValue("face")]
    FaceRecognition,
    
    [EnumValue("eye")]
    IrisScan,
    
    [EnumValue("federated")]
    Federated,
    
    [EnumValue("social")]
    Social,
    
    [EnumValue("email")]
    EmailVerification
}