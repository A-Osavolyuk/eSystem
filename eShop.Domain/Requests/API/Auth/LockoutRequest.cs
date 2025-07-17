namespace eShop.Domain.Requests.API.Auth;

public record LockoutRequest
{
    public Guid UserId { get; set; }
}