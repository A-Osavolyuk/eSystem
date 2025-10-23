using eSystem.Domain.Enums;

namespace eSystem.Domain.Requests.Auth;

public record ChangePhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required PhoneNumberType Type { get; set; }
    public required string NewPhoneNumber { get; set; }
}