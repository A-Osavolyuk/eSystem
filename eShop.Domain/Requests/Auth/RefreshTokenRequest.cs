namespace eShop.Domain.Requests.Auth;

public record RefreshTokenRequest
{
    public Guid UserId { get; set; }
}