namespace eShop.Auth.Api.Entities;

public class OAuthSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ProviderId { get; set; }
    
    public bool IsSucceeded { get; set; }
    public string? ErrorMessage { get; set; }
    public ErrorCode ErrorCode { get; set; }

    public OAuthSignType SignType { get; set; }
    public DateTimeOffset? ExpiredDate { get; set; }

    public UserEntity? User { get; set; }
    public OAuthProviderEntity? Provider { get; set; }
}