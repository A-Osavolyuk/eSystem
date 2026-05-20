using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class UserRoleEntity : Entity
{
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    
    public Guid RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;

}