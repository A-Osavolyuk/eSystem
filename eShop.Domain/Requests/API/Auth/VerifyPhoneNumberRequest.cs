namespace eShop.Domain.Requests.API.Auth;

public class VerifyPhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}