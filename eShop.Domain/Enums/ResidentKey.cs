using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace eShop.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResidentKey
{
    [EnumMember(Value = "required")]
    Required,
    
    [EnumMember(Value = "preferred")]
    Preferred,
    
    [EnumMember(Value = "discouraged")]
    Discouraged
}