namespace eShop.Domain.Requests.API.Auth;

public record ChangeUserNameRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}