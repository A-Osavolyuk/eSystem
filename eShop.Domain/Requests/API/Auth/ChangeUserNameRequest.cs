namespace eShop.Domain.Requests.API.Auth;

public record ChangeUserNameRequest
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
}