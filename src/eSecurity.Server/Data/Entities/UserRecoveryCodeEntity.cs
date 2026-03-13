using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class UserRecoveryCodeEntity : Entity
{
    public Guid Id { get; set; }
    
    public string ProtectedCode { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}