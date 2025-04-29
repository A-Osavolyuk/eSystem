namespace eShop.Domain.Requests.API.Auth;

public record ExternalLoginRequest
{
    public string Provider { get; set; } = string.Empty;
    public string ReturnUri { get; set; } = string.Empty;
}