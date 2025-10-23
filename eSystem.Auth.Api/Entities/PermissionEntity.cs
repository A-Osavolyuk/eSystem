namespace eSystem.Auth.Api.Entities;

public class PermissionEntity : Entity
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid ResourceId { get; set; }
    public ResourceEntity Resource { get; init; } = null!;
}