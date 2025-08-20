using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace eShop.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserVerification
{
    [EnumMember(Value = "preferred")]
    Required,
    
    [EnumMember(Value = "preferred")]
    Preferred,
    
    [EnumMember(Value = "discouraged")]
    Discouraged
}