namespace eShop.Domain.Requests.API.Auth;

public class UnsubscribeProviderRequest
{
    public Guid Id { get; set; }
    public string Provider { get; set; } = string.Empty;
}