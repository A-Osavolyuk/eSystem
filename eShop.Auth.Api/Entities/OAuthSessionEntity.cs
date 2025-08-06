namespace eShop.Auth.Api.Entities;

public class OAuthSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    public string Provider { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public OAuthSignType SignType { get; set; }
    
    public DateTimeOffset? ExpiredDate { get; set; }
}