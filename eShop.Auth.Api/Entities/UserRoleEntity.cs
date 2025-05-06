namespace eShop.Auth.Api.Entities;

public class UserRoleEntity : IdentityUserRole<Guid>, IEntity
{
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public UserEntity User { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;
}