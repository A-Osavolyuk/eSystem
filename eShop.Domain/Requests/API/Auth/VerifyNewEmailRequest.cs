namespace eShop.Domain.Requests.API.Auth;

public class VerifyNewEmailRequest
{
    public Guid UserId { get; set; }
    public string NewEmail { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}