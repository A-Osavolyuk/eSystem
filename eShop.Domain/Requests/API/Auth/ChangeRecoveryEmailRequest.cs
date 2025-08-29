namespace eShop.Domain.Requests.API.Auth;

public class ChangeRecoveryEmailRequest
{
    public Guid UserId { get; set; }
    public string NewRecoveryEmail { get; set; } = string.Empty;
}