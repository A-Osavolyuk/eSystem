using eSystem.Core.Enums;

namespace eSecurity.Server.Security.Authorization.Codes;

public enum CodeState
{
    [EnumValue("pending")]
    Pending,
    
    [EnumValue("consumed")]
    Consumed,
    
    [EnumValue("cancelled")]
    Cancelled
}