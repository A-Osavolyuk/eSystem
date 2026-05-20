using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class UserRecoveryCodeEntity : Entity
{
    public Guid Id { get; set; }
    
    public string ProtectedCode { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}