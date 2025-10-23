namespace eSystem.Domain.Requests.Auth;

public class ResetPasswordRequest
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}