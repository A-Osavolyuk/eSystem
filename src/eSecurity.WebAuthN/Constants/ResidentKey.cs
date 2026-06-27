using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.WebAuthN.Constants;

[JsonConverter(typeof(JsonEnumValueConverter<ResidentKey>))]
public enum ResidentKey
{
    [EnumValue("required")]
    Required,
    
    [EnumValue("preferred")]
    Preferred,
    
    [EnumValue("discouraged")]
    Discouraged
}