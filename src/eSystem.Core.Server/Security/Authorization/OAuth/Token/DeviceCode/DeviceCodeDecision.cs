using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSystem.Core.Server.Security.Authorization.OAuth.Token.DeviceCode;

[JsonConverter(typeof(JsonEnumValueConverter<DeviceCodeDecision>))]
public enum DeviceCodeDecision
{
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied
}