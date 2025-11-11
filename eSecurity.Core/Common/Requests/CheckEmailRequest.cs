namespace eSecurity.Core.Common.Requests;

public class CheckEmailRequest
{
    public Guid UserId { get; set; }
    public required string Email { get; set; }
}