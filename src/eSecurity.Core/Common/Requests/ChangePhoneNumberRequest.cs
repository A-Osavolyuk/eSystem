using eSecurity.Core.Security.Identity;

namespace eSecurity.Core.Common.Requests;

public class ChangePhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string NewPhoneNumber { get; set; }
    public required PhoneNumberType Type { get; set; }
}