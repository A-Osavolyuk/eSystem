using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Attestation;

[JsonConverter(typeof(JsonEnumValueConverter<AttestationTrustType>))]
public enum AttestationTrustType
{
    [EnumValue("none")]
    None,
    
    [EnumValue("untrusted")]
    Untrusted,
    
    [EnumValue("trusted")]
    Trusted
}