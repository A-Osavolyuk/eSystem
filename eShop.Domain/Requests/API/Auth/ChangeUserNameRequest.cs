namespace eShop.Domain.Requests.Api.Auth;

public record class ChangeUserNameRequest
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}