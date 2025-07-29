namespace eShop.Auth.Api.Entities;

public class UserProviderEntity : Entity
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    public bool Subscribed { get; set; }
    
    public DateTimeOffset? SubscribedDate { get; set; }
    public DateTimeOffset? UnsubscribedDate { get; set; }
    
    public UserEntity User { get; set; } = null!;
    public ProviderEntity Provider { get; set; } = null!;
}