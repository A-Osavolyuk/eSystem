namespace eShop.Domain.Requests.API.Auth;

public class EnableTwoFactorRequest
{
    public Guid UserId { get; set; }
}