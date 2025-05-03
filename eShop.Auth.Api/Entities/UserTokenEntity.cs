namespace eShop.Auth.Api.Entities;

public class UserTokenEntity : IdentityUserToken<Guid>, IEntity
{
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}