namespace eShop.BlazorWebUI.Models;

public class ConfirmResetEmailModel
{
    public Guid Id { get; set; }
    public string NewEmail { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}