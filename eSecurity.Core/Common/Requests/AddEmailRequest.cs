namespace eSecurity.Core.Common.Requests;

public class AddEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}