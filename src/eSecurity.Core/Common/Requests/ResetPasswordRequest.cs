namespace eSecurity.Core.Common.Requests;

public sealed class ResetPasswordRequest
{
    public required string Subject { get; set; }
    public required string NewPassword { get; set; }
}