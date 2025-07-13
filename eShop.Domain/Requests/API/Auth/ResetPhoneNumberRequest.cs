namespace eShop.Domain.Requests.API.Auth;

public class ResetPhoneNumberRequest
{
    public Guid UserId { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
}