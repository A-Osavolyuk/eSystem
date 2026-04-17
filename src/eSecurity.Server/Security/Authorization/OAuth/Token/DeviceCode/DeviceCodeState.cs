using System.Text.Json.Serialization;
using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

[JsonConverter(typeof(JsonEnumValueConverter<DeviceCodeState>))]
public enum DeviceCodeState
{
    [EnumValue("pending")]
    Pending,
    
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied,
    
    [EnumValue("consumed")]
    Consumed,
    
    [EnumValue("expired")]
    Expired
}