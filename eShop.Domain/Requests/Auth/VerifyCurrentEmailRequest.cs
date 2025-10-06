namespace eShop.Domain.Requests.Auth;

public class VerifyCurrentEmailRequest
{
    public Guid UserId { get; set; }
    public string NewEmail { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}