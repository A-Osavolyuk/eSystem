using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class RoleEntity : Entity
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public ICollection<UserRoleEntity> Roles { get; set; } = null!;
}