using eSecurity.Core.Security.Identity;

namespace eSecurity.Core.Common.DTOs;

public class UserPhoneNumberDto
{
    public Guid Id { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;
    public PhoneNumberType Type { get; set; }
    public bool IsVerified { get; set; }

    public DateTimeOffset? VerifiedDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}