namespace eShop.Domain.Requests.API.Auth;

public class CheckPhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}