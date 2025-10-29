using eSystem.Core.Data.Entities;

namespace eSystem.Auth.Api.Data.Entities;

public class RolePermissionEntity : Entity
{
    public Guid PermissionId { get; set; }
    public Guid RoleId { get; set; }
    public PermissionEntity Permission { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
}