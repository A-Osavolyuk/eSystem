namespace eShop.Domain.Requests.API.Auth;

public class ConfirmRemoveRecoveryEmailRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}