namespace eShop.Domain.Requests.Auth;

public class VerifyCurrentPhoneNumberRequest
{
    public Guid UserId { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}