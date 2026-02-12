using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class GrantTypeEntity : Entity
{
    public Guid Id { get; set; }

    public required string Grant { get; set; }
}