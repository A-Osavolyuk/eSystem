using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class PasswordEntity : Entity
{
    public Guid Id { get; set; }
    
    public string Hash { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}