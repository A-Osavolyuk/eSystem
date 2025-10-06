namespace eShop.Domain.Requests.Auth;

public class RemovePasskeyRequest
{
    public Guid UserId { get; set; }
    public Guid KeyId { get; set; }
}