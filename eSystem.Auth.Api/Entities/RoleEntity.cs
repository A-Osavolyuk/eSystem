namespace eSystem.Auth.Api.Entities;

public class RoleEntity : Entity
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public ICollection<UserRoleEntity> Roles { get; set; } = null!;
    public ICollection<RolePermissionEntity> Permissions { get; set; } = null!;
}