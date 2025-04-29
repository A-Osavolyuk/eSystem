namespace eShop.Domain.Requests.API.Auth;

public record LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
