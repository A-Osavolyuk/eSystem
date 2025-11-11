namespace eSecurity.Core.Common.Requests;

public class ResetPasswordRequest
{
    public required Guid UserId { get; set; }
    public required string NewPassword { get; set; }
}