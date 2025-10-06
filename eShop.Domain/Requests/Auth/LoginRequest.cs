namespace eShop.Domain.Requests.Auth;

public record LoginRequest
{
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
}
