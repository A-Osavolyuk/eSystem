namespace eShop.Domain.Requests.API.Auth;

public class ConfirmRemovePasskeyRequest
{
    public Guid UserId { get; set; }
    public Guid KeyId { get; set; }
    public string Code { get; set; } = string.Empty;
}