namespace eShop.Auth.Api.Entities;

public class PermissionEntity : IAuditable, IIdentifiable<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreateDate { get; init; }
    public DateTime UpdateDate { get; init; }
    
    [JsonIgnore] public ICollection<UserPermissionsEntity> Permissions { get; init; } = null!;
}