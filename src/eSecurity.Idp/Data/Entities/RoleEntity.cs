using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class RoleEntity : Entity
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public ICollection<UserRoleEntity> Roles { get; set; } = null!;
}