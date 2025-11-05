namespace eSystem.Core.Requests.Auth;

public class LogoutRequest
{
    public required Guid SessionId { get; set; }
}