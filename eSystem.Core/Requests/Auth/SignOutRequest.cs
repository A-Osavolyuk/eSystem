namespace eSystem.Core.Requests.Auth;

public class SignOutRequest
{
    public required Guid UserId { get; set; }
}