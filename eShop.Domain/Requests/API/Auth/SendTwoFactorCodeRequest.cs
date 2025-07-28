namespace eShop.Domain.Requests.API.Auth;

public class SendTwoFactorCodeRequest
{
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
}