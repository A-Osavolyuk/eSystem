namespace eSecurity.Core.Common.Requests;

public class AddPasswordRequest
{
    public required Guid UserId { get; set; }
    public required string Password { get; set; }
}