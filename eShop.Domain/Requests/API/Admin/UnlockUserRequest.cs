namespace eShop.Domain.Requests.Api.Admin;

public record UnlockUserRequest
{
    public Guid UserId { get; set; }
}