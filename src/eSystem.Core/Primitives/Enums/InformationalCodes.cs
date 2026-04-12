using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Primitives.Enums;

[JsonConverter(typeof(JsonEnumValueConverter<InformationalCodes>))]
public enum InformationalCodes
{
    [EnumValue("continue")]
    Continue = 100,
    
    [EnumValue("switching_protocols")]
    SwitchingProtocols = 101,
    
    [EnumValue("processing")]
    Processing = 102,
    
    [EnumValue("early_hints")]
    EarlyHints = 103
}