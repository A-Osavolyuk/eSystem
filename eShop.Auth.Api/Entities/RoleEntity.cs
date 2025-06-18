namespace eShop.Auth.Api.Entities;

public class RoleEntity : IEntity
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    
    public ICollection<UserRoleEntity> Roles { get; set; } = null!;
    public ICollection<RolePermissionEntity> Permissions { get; set; } = null!;
}