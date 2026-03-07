using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class UserSecretEntity : Entity
{
    public Guid Id { get; init; }
    public required string ProtectedSecret { get; set; }
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}