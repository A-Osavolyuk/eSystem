namespace eShop.Domain.Requests.Api.Auth;

public record class ChangeEmailRequest
{
    public string CurrentEmail { get; set; } = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
}