namespace eShop.Domain.Requests.API.Auth;

public record ConfirmChangeEmailRequest
{
    public string CurrentEmail { get; set; } = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
    public CodeSet CodeSet { get; set; } = null!;
}