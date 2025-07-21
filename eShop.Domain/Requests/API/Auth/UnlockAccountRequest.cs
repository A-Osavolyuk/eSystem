namespace eShop.Domain.Requests.API.Auth;

public class UnlockAccountRequest
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}