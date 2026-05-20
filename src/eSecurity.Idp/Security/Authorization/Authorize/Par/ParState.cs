using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSecurity.Idp.Security.Authorization.Authorize.Par;

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