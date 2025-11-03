using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class UserRecoveryCodeEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    public string ProtectedCode { get; set; } = string.Empty;
    
    public UserEntity User { get; set; } = null!;
}