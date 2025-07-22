namespace eShop.Domain.Requests.API.Auth;

public class VerifyNewPhoneNumberRequest
{
    public Guid UserId { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}