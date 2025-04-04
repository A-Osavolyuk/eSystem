namespace eShop.Domain.Requests.Api.Auth;

public record class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
