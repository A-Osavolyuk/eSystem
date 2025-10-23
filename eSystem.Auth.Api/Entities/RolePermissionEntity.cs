namespace eSystem.Auth.Api.Entities;

public class RolePermissionEntity : Entity
{
    public Guid PermissionId { get; set; }
    public Guid RoleId { get; set; }
    public PermissionEntity Permission { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
}