namespace eShop.Auth.Api.Entities;

public class UserTwoFactorProviderEntity : Entity
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    public bool Subscribed { get; set; }
    
    public UserEntity User { get; set; } = null!;
    public TwoFactorProviderEntity TwoFactorProvider { get; set; } = null!;
}