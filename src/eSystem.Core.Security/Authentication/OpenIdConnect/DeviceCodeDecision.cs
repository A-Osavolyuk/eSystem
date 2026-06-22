using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Security.Authentication.OpenIdConnect;

[JsonConverter(typeof(JsonEnumValueConverter<DeviceCodeDecision>))]
public enum DeviceCodeDecision
{
    [EnumValue("none")]
    None,
    
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied
}