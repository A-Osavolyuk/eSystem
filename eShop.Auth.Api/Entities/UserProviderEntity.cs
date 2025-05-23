namespace eShop.Auth.Api.Entities;

public class UserProviderEntity : IEntity
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    
    public DateTimeOffset? CreateDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
    
    public UserEntity User { get; set; } = null!;
    public ProviderEntity Provider { get; set; } = null!;
}