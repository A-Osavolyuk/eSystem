using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Http;

[JsonConverter(typeof(JsonEnumValueConverter<AuthenticationType>))]
public enum AuthenticationType
{
    [EnumValue("none")]
    None,
    
    [EnumValue("bearer")]
    Bearer,
    
    [EnumValue("basic")]
    Basic
}