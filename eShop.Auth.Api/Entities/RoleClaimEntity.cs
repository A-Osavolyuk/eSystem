namespace eShop.Auth.Api.Entities;

public class RoleClaimEntity : IdentityRoleClaim<Guid>, IEntity
{
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}