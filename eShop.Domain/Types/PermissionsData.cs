using eShop.Domain.Abstraction.Data;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace eShop.Domain.Types;

public record PermissionsData : IIdentifiable<Guid>
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public List<RoleDto> Roles { get; set; } = [];
    public List<PermissionDto> Permissions { get; set; } = [];
}