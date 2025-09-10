namespace eShop.Domain.Responses.API.Auth;

public class CheckAccountResponse
{
    public Guid UserId { get; set; }
    public bool IsLockedOut { get; set; }
    public bool Exists { get; set; }
    
    public bool HasRecoveryEmail { get; set; }
    public string? RecoveryEmail { get; set; } = string.Empty;
}