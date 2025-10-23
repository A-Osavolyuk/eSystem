namespace eSystem.Domain.Requests.Auth;

public class ChangePasskeyNameRequest
{
    public required Guid UserId { get; set; }
    public required Guid PasskeyId { get; set; }
    public required string DisplayName { get; set; }
}