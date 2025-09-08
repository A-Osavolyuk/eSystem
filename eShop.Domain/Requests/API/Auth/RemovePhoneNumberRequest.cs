namespace eShop.Domain.Requests.API.Auth;

public class RemovePhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}