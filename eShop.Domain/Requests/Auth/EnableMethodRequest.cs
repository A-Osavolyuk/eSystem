namespace eShop.Domain.Requests.Auth;

public class EnableMethodRequest
{
    public Guid UserId { get; set; }
    public LoginType Type { get; set; }
}