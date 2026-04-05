using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueStringConverter<ResponseTokenType>))]
public enum ResponseTokenType
{
    [EnumValue("Bearer")]
    Bearer,
    
    [EnumValue("MAC")]
    Mac,
    
    [EnumValue("DPoP")]
    DPoP
}