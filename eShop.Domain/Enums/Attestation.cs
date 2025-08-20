using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace eShop.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Attestation
{
    [EnumMember(Value = "none")]
    None,
    
    [EnumMember(Value = "direct")]
    Direct,
    
    [EnumMember(Value = "indirect")]
    Indirect
}