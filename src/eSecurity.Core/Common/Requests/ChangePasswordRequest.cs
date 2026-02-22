namespace eSecurity.Core.Common.Requests;

public sealed class ChangePasswordRequest
{
    public required string Subject { get; set; }
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmNewPassword { get; set; }
}