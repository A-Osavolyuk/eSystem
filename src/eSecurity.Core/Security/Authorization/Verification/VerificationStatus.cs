using eSystem.Core.Enums;
using eSystem.Core.Enums.Serialization;

namespace eSecurity.Core.Security.Authorization.Verification;

[JsonConverter(typeof(JsonEnumValueConverter<VerificationStatus>))]
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