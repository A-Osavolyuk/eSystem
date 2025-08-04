namespace eShop.Auth.Api.Entities;

public class UserOAuthProviderEntity
{
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }

    public UserEntity User { get; set; } = null!;
    public OAuthProviderEntity Provider { get; set; } = null!;
}