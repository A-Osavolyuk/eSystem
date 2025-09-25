namespace eShop.Domain.Requests.API.Auth;

public class DisableTwoFactorRequest
{
    public Guid UserId { get; set; }
}