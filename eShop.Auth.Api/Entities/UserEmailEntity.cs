namespace eShop.Auth.Api.Entities;

public class UserEmailEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public EmailType Type { get; set; }
    public bool IsVerified { get; set; }

    public DateTimeOffset? VerifiedDate { get; set; }

    public UserEntity User { get; set; } = null!;
}