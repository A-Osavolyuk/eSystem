namespace eShop.Auth.Api.Entities;

public class RoleEntity : IdentityRole<Guid>, IEntity
{
    public DateTime CreateDate { get; set; }
    public DateTime UpdateDate { get; set; }
}