namespace eSecurity.Core.Common.Requests;

public class RemovePhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}