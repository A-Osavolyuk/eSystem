using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.WebAuthN.Constants;

[JsonConverter(typeof(JsonEnumValueConverter<Attestation>))]
public enum Attestation
{
    [EnumValue("none")]
    None,
    
    [EnumValue("direct")]
    Direct,
    
    [EnumValue("indirect")]
    Indirect,
    
    [EnumValue("enterprise")]
    Enterprise
}