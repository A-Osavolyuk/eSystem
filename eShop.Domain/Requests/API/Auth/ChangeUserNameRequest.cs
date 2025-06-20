namespace eShop.Domain.Requests.API.Auth;

public record ChangeUserNameRequest
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
}