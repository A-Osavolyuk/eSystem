namespace eShop.Domain.Requests.API.Auth;

public record Change2FaStateRequest
{
    public string Email { get; set; } = string.Empty;
}