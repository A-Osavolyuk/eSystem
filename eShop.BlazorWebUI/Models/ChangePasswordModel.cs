namespace eShop.BlazorWebUI.Models;

public class ChangePasswordModel
{
    public string Email { get; set; } = string.Empty;
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}