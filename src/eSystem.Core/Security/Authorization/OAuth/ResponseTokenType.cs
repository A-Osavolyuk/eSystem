using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authorization.OAuth;

[JsonConverter(typeof(JsonEnumValueConverter<ResponseTokenType>))]
public enum ResponseTokenType
{
    [EnumValue("Bearer")]
    Bearer,
    
    [EnumValue("MAC")]
    Mac,
    
    [EnumValue("DPoP")]
    DPoP
}