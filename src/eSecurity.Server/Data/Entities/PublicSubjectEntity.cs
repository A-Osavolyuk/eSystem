using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class PublicSubjectEntity : Entity
{
    public Guid Id { get; set; }
    public required string Subject { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}