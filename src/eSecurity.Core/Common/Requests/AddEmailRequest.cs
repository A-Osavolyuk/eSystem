namespace eSecurity.Core.Common.Requests;

public sealed class AddEmailRequest
{
    public required string Subject { get; set; }
    public required string Email { get; set; }
}