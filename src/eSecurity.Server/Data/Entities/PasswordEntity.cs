using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class PasswordEntity : Entity
{
    public Guid Id { get; set; }
    
    public string Hash { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}