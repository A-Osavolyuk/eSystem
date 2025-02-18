namespace eShop.Auth.Api.Entities;

public class SecurityTokenEntity
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiredAt { get; init; }

    public AppUser User { get; init; } = null!;
}