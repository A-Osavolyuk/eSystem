namespace eShop.Auth.Api.Entities;

public class UserClaimEntity : IdentityUserClaim<Guid>, IEntity
{
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}