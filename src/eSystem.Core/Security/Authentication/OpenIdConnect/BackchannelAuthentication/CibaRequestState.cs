using eSystem.Core.Enums;

namespace eSystem.Core.Security.Authentication.OpenIdConnect.BackchannelAuthentication;

public enum CibaRequestState
{
    [EnumValue("pending")]
    Pending,
    
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("denied")]
    Denied,
    
    [EnumValue("expired")]
    Expired,
    
    [EnumValue("consumed")]
    Consumed,
    
    [EnumValue("cancelled")]
    Cancelled
}