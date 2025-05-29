namespace eShop.Domain.Requests.API.Auth;

public class UnsubscribeProviderRequest
{
    public string Email { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}