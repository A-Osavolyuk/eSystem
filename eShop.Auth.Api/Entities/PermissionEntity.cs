using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Entities;

public class PermissionEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    
    public ICollection<UserPermissionsEntity> Permissions { get; init; } = null!;
}