namespace eShop.Domain.Requests.API.Auth;

public class DisableMethodRequest
{
    public Guid UserId { get; set; }
    public LoginType Type { get; set; }
}