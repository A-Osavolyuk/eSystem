using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

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
    Expired,
    
    [EnumValue("cancelled")]
    Cancelled
}