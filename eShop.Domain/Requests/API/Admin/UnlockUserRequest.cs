namespace eShop.Domain.Requests.API.Admin;

public record UnlockUserRequest
{
    public Guid UserId { get; set; }
}