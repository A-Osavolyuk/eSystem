namespace eSecurity.Core.Common.Requests;

public class RemoveEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}