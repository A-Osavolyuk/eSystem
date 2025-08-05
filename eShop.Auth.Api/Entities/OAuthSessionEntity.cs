namespace eShop.Auth.Api.Entities;

public class OAuthSessionEntity : Entity
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }

    public string Provider { get; set; } = string.Empty;
    public bool IsSucceeded { get; set; }
    public string? ErrorMessage { get; set; }
    public OAuthErrorType ErrorType { get; set; }

    public OAuthSignType SignType { get; set; }
    public DateTimeOffset? ExpiredDate { get; set; }
}