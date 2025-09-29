namespace eShop.Domain.Requests.API.Auth;

public class EnableMethodRequest
{
    public Guid UserId { get; set; }
    public LoginType Type { get; set; }
}