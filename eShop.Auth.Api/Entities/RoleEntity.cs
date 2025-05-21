namespace eShop.Auth.Api.Entities;

public class RoleEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    
    public ICollection<UserRoleEntity> Roles { get; set; } = null!;
    public ICollection<RolePermissionEntity> Permissions { get; set; } = null!;
}