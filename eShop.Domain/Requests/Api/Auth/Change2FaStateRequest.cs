namespace eShop.Domain.Requests.Api.Auth;

public record class Change2FaStateRequest
{
    public string Email { get; set; } = string.Empty;
}