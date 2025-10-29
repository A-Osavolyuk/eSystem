using eSystem.Core.Data.Entities;

namespace eSystem.Auth.Api.Data.Entities;

public class UserRoleEntity : Entity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public UserEntity User { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;

}