namespace eShop.BlazorWebUI.Models;

public class ConfirmChangeEmailModel
{
    public Guid UserId { get; set; }
    public string CurrentEmailCode { get; set; } = string.Empty;
    public string NewEmailCode { get; set; } = string.Empty;
    public string NewEmail { get; set; } = string.Empty;
}