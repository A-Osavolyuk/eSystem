namespace eShop.Auth.Api.Entities;

public class PermissionEntity : IAuditable, IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
    
    [JsonIgnore] public ICollection<UserPermissionsEntity> Permissions { get; init; } = null!;
}