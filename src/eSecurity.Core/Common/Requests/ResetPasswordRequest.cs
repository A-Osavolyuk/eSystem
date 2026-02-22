namespace eSecurity.Core.Common.Requests;

public sealed class ResetPasswordRequest
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}