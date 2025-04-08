using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace eShop.Domain.Types;

public record PermissionsData : IIdentifiable<Guid>
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public List<RoleData> Roles { get; set; } = new();
    public List<Permission> Permissions { get; set; } = new();
}