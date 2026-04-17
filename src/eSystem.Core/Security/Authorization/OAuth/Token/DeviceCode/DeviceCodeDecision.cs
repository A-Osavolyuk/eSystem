using System.Text.Json.Serialization;
using eSystem.Core.Data.Conversion;
using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authorization.OAuth.Token.DeviceCode;

[JsonConverter(typeof(EnumValueConverter<DeviceCodeDecision>))]
public enum DeviceCodeDecision
{
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied
}