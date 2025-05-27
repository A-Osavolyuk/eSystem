namespace eShop.Domain.Requests.API.Admin;

public record UnlockRequest
{
    public Guid UserId { get; set; }
}