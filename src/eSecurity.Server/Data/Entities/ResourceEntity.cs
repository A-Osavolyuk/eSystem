using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class ResourceEntity : Entity
{
    public Guid Id { get; init; }
    public Guid OwnerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public ResourceOwnerEntity Owner { get; set; } = null!;
}