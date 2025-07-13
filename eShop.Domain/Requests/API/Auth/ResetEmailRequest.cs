namespace eShop.Domain.Requests.API.Auth;

public class ResetEmailRequest
{
    public Guid UserId { get; set; }
    public string NewEmail { get; set; } = string.Empty;
}