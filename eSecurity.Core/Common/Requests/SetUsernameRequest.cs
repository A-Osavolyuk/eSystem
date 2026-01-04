namespace eSecurity.Core.Common.Requests;

public class SetUsernameRequest
{
    public required Guid UserId { get; set; }
    public required string Username { get; set; }
}