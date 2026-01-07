using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class SignInSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public DateTimeOffset ExpireDate { get; set; }
    public bool IsActive => ExpireDate > DateTimeOffset.UtcNow;

    public UserEntity User { get; set; } = null!;
}