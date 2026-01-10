namespace eSecurity.Core.Common.Requests;

public class VerifyPhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}