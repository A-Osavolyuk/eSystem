namespace eShop.Domain.Requests.API.Auth;

public class ResetEmailRequest
{
    public Guid Id { get; set; }
    public string NewEmail { get; set; } = string.Empty;
}