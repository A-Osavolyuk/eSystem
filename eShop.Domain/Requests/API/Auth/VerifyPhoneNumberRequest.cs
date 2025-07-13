namespace eShop.Domain.Requests.API.Auth;

public class VerifyPhoneNumberRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}