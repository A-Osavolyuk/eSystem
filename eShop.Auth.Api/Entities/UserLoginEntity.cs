namespace eShop.Auth.Api.Entities;

public class UserLoginEntity : IdentityUserLogin<Guid>, IEntity
{
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
}