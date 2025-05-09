namespace eShop.Domain.Requests.API.TwoFactor;

public class SubscribeProviderRequest
{
    public string Email { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}