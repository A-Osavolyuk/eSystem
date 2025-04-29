namespace eShop.Domain.Requests.API.Admin;

public record DeleteUserAccountRequest
{
    public Guid UserId { get; set; }
}