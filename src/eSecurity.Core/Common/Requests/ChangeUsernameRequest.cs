namespace eSecurity.Core.Common.Requests;

public class ChangeUsernameRequest
{
    public required Guid UserId { get; set; }
    public required string Username { get; set; }
}