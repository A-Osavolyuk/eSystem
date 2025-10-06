namespace eShop.Domain.Requests.Auth;

public class VerifyProviderRequest
{
    public Guid UserId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}