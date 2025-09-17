namespace eShop.Blazor.Server.Domain.Models;

public class ChangePasswordModel
{
    public Guid Id { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmNewPassword { get; set; } = string.Empty;
}