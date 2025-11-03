using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSecurity.Data.Entities;

public class UserPhoneNumberEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string PhoneNumber { get; set; } = string.Empty;
    public PhoneNumberType Type { get; set; }
    public bool IsVerified { get; set; }
    
    public DateTimeOffset? VerifiedDate { get; set; }
    
    public UserEntity User { get; set; } = null!;
}