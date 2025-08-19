namespace eShop.Auth.Api.Entities;

public class UserLinkedAccountEntity : Entity
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    public bool Allowed { get; set; }

    public UserEntity User { get; set; } = null!;
    public OAuthProviderEntity Provider { get; set; } = null!;
}