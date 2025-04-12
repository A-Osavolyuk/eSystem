namespace eShop.Domain.Requests.API.Auth;

public record class Change2FaStateRequest
{
    public string Email { get; set; } = string.Empty;
}