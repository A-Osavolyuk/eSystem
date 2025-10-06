namespace eShop.Domain.Requests.Auth;

public record UnlockRequest
{
    public Guid UserId { get; set; }
}