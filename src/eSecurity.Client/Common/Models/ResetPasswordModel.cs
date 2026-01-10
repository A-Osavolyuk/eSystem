namespace eSecurity.Client.Common.Models;

public class ResetPasswordModel
{
    public Guid Id { get; set; }
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}