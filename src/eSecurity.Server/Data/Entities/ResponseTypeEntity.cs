using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class ResponseTypeEntity : Entity
{
    public Guid Id { get; set; }
    public required string Type { get; set; }
}