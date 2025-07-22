namespace eShop.Domain.Requests.API.Auth;

public record LoginRequest
{
    public string Login { get; set; } = "";
    public string Password { get; set; } = "";
}
