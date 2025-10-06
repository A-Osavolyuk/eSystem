namespace eShop.Domain.Requests.Auth;

public record ChangeTwoFactorStateRequest
{
    public Guid UserId { get; set; }
}