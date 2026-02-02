using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class SessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public DateTimeOffset? ExpireDate { get; set; }

    public UserEntity User { get; set; } = null!;
    public ICollection<OpaqueTokenEntity> OpaqueTokens { get; set; } = null!;
}