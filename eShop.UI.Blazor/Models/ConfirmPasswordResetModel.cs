namespace eShop.BlazorWebUI.Models;

public class ConfirmPasswordResetModel
{
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}