using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class ScopeEntity : Entity
{
    public required Guid Id { get; set; }
    public required string Description { get; set; }
    public required string Value { get; set; }
}