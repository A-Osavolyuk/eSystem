using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class PublicSubjectEntity : Entity
{
    public Guid Id { get; set; }
    public required string Subject { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}