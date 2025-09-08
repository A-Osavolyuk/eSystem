namespace eShop.Domain.Requests.API.Auth;

public class ResetPhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
    public required string NewPhoneNumber { get; set; }
}