using System.Text.Json.Serialization;
using eShop.Auth.Api.Enums;

namespace eShop.Auth.Api.Entities;

public class PermissionEntity : IEntity<Guid>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid ResourceId { get; set; }
    public ActionType Action { get; set; }
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    
    public ResourceEntity Resource { get; init; } = null!;
}