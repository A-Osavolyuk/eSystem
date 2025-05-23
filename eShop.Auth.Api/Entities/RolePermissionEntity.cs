namespace eShop.Auth.Api.Entities;

public class RolePermissionEntity : IEntity
{
    public Guid PermissionId { get; set; }
    public Guid RoleId { get; set; }
    
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }

    public PermissionEntity Permission { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
}