namespace eShop.Domain.Requests.API.Auth;

public class AddRecoveryEmailRequest
{
    public Guid UserId { get; set; }
    public string RecoveryEmail { get; set; } = string.Empty;
}