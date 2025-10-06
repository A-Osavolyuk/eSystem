namespace eShop.Domain.Requests.Auth;

public record DeleteUserAccountRequest
{
    public Guid UserId { get; set; }
}