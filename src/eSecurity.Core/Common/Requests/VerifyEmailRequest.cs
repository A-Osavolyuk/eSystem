namespace eSecurity.Core.Common.Requests;

public sealed class VerifyEmailRequest
{
    public required string Subject { get; set; }
    public required string Email { get; set; }
}