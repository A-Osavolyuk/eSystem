using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResidentKey
{
    [EnumMember(Value = "preferred")]
    Required,
    
    [EnumMember(Value = "preferred")]
    Preferred,
    
    [EnumMember(Value = "discouraged")]
    Discouraged
}