namespace eShop.Domain.Requests.API.Auth;

public class AddPhoneNumberRequest
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; } = string.Empty;
    public required PhoneNumberType Type { get; set; }
}