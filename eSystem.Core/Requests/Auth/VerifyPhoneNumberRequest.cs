namespace eSystem.Core.Requests.Auth;

public class VerifyPhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}