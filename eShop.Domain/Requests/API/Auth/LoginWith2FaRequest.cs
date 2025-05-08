namespace eShop.Domain.Requests.API.Auth;

public record LoginWith2FaRequest
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}