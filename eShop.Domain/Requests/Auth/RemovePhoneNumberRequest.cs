namespace eShop.Domain.Requests.Auth;

public class RemovePhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}