using eSystem.Domain.Enums;

namespace eSystem.Domain.DTOs;

public class UserPhoneNumberDto
{
    public Guid Id { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;
    public PhoneNumberType Type { get; set; }
    public bool IsVerified { get; set; }

    public DateTimeOffset? VerifiedDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}