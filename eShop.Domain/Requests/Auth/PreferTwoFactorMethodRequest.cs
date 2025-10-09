namespace eShop.Domain.Requests.Auth;

public class PreferTwoFactorMethodRequest
{
    public Guid UserId { get; set; }
    public TwoFactorMethod PreferredMethod { get; set; }
}