namespace eShop.Auth.Api.Entities;

public class UserEmailEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    
    public bool IsVerified { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsRecovery { get; set; }

    public DateTimeOffset? VerifiedDate { get; set; }
    public DateTimeOffset? PrimaryDate { get; set; }
    public DateTimeOffset? RecoveryDate { get; set; }

    public UserEntity User { get; set; } = null!;
}