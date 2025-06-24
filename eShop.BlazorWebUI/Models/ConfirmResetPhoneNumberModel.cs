namespace eShop.BlazorWebUI.Models;

public class ConfirmResetPhoneNumberModel
{
    public Guid Id { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}