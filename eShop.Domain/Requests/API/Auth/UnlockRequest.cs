namespace eShop.Domain.Requests.API.Auth;

public record UnlockRequest
{
    public Guid UserId { get; set; }
}