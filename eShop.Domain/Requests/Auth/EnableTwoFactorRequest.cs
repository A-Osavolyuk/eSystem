namespace eShop.Domain.Requests.Auth;

public class EnableTwoFactorRequest
{
    public Guid UserId { get; set; }
}