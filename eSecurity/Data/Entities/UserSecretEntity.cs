using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class UserSecretEntity : Entity
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }
    public string Secret { get; set; } = string.Empty;
    public UserEntity User { get; set; } = null!;
}