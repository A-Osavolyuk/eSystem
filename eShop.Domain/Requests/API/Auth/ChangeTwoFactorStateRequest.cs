namespace eShop.Domain.Requests.API.Auth;

public record ChangeTwoFactorStateRequest
{
    public Guid UserId { get; set; }
}