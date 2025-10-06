namespace eShop.Domain.Requests.Auth;

public class ResetPhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string NewPhoneNumber { get; set; }
}