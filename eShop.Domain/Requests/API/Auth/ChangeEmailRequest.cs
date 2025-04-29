namespace eShop.Domain.Requests.API.Auth;

public record ChangeEmailRequest
{
    public string CurrentEmail { get; set; } = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
}