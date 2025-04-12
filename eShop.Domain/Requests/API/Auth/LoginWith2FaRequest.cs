namespace eShop.Domain.Requests.API.Auth;

public record class LoginWith2FaRequest
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}