using eSecurity.Core.Security.Identity;

namespace eSecurity.Core.Common.Requests;

public class AddPhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
    public required PhoneNumberType Type { get; set; }
}