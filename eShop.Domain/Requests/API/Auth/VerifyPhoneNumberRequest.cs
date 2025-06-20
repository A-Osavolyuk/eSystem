namespace eShop.Domain.Requests.API.Auth;

public class VerifyPhoneNumberRequest
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
}