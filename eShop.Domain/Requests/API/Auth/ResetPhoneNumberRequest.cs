namespace eShop.Domain.Requests.API.Auth;

public class ResetPhoneNumberRequest
{
    public Guid UserId { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
}