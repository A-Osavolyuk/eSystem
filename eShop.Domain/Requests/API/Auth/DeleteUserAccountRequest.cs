namespace eShop.Domain.Requests.API.Auth;

public record DeleteUserAccountRequest
{
    public Guid UserId { get; set; }
}