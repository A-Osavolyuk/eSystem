using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class UserPermissionsEntity : Entity
{
    public Guid PermissionId { get; init; }
    public Guid UserId { get; init; }
    public UserEntity User { get; set; } = null!;
    public PermissionEntity Permission { get; set; } = null!;
}