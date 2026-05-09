using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.Par;

[JsonConverter(typeof(JsonNumberEnumConverter<ParState>))]
public enum ParState
{
    [EnumValue("pending")]
    Pending,
    
    [EnumValue("consumed")]
    Consumed,
    
    [EnumValue("expired")]
    Expired,
    
    [EnumValue("cancelled")]
    Cancelled
}