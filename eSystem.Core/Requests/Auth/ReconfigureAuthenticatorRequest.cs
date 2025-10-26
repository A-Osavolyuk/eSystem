namespace eSystem.Core.Requests.Auth;

public class ReconfigureAuthenticatorRequest
{
    public required Guid UserId { get; set; }
    public required string Secret { get; set; }
}