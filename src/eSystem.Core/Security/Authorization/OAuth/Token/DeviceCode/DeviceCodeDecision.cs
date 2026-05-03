using System.Text.Json.Serialization;
using eSystem.Core.Data.Conversion;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

[JsonConverter(typeof(JsonEnumValueConverter<DeviceCodeDecision>))]
public enum DeviceCodeDecision
{
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied
}