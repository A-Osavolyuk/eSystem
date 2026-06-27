using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation;

[JsonConverter(typeof(JsonEnumValueConverter<AttestationFormatType>))]
public enum AttestationFormatType
{
    [EnumValue("none")]
    None,
    
    [EnumValue("packed")]
    Packed,
        
    [EnumValue("fido-u2f")]
    FidoU2F,
        
    [EnumValue("enterprise")]
    Enterprise,
        
    [EnumValue("apple")]
    Apple,
        
    [EnumValue("android-key")]
    AndroidKey,
    
    [EnumValue("android-safetynet")]
    AndroidSafetyNet,
        
    [EnumValue("tpm")]
    Tpm,
}