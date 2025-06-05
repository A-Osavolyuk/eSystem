namespace eShop.Domain.Requests.API.Auth;

public record ChangeEmailRequest
{
    public Guid UserId { get; set; }
    public string NewEmail { get; set; } = string.Empty;
}