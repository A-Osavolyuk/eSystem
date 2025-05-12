namespace eShop.Auth.Api.Entities;

public class RolePermission : IEntity
{
    public Guid PermissionId { get; set; }
    public Guid RoleId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public PermissionEntity Permission { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
}