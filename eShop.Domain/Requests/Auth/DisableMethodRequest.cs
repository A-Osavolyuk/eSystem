namespace eShop.Domain.Requests.Auth;

public class DisableMethodRequest
{
    public Guid UserId { get; set; }
    public LoginType Type { get; set; }
}