namespace eShop.Domain.Requests.API.Auth;

public class RemovePasskeyRequest
{
    public Guid UserId { get; set; }
    public Guid KeyId { get; set; }
}