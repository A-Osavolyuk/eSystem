namespace eShop.Auth.Api.Entities;

public class UserProviderEntity : IEntity
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    
    public UserEntity User { get; set; } = null!;
    public ProviderEntity Provider { get; set; } = null!;
}