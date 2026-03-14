using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authorization.Verification;

public enum VerificationStatus
{
    [EnumValue("pending")]
    Pending,
    
    [EnumValue("approved")]
    Approved,
    
    [EnumValue("consumed")]
    Consumed,
    
    [EnumValue("expired")]
    Expired,
    
    [EnumValue("cancelled")]
    Cancelled
}