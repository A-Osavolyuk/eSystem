namespace eShop.Auth.Api.Entities;

public class UserRoleEntity : IEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    public UserEntity User { get; set; } = null!;
    public RoleEntity Role { get; set; } = null!;

}