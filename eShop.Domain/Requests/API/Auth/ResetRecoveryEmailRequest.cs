namespace eShop.Domain.Requests.API.Auth;

public class ResetRecoveryEmailRequest
{
    public Guid UserId { get; set; }
    public string NewRecoveryEmail { get; set; } = string.Empty;
}