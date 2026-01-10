namespace eSecurity.Core.Common.Requests;

public class ResetEmailRequest
{
    public required Guid UserId { get; set; }
    public required string NewEmail { get; set; }
}