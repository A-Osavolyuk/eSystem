namespace eShop.Domain.Requests.Auth;

public class SetPasskeyNameRequest
{
    public required Guid UserId { get; set; }
    public required Guid PasskeyId { get; set; }
    public required string DisplayName { get; set; }
}