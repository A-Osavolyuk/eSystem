namespace eShop.Auth.Api.Entities;

public class PermissionEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid ResourceId { get; set; }
    public ActionType Action { get; set; }
    
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    
    public ResourceEntity Resource { get; init; } = null!;
}