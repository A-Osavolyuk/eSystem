namespace eSecurity.Core.Common.Requests;

public class VerifyEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}