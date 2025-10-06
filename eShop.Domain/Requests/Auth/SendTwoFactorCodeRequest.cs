namespace eShop.Domain.Requests.Auth;

public class SendTwoFactorCodeRequest
{
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
}