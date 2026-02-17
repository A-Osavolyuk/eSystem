namespace eSecurity.Core.Common.Requests;

public class ResetEmailRequest
{
    public required string Subject { get; set; }
    public required string NewEmail { get; set; }
}