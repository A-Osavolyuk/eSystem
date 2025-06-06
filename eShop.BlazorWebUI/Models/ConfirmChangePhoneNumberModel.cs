namespace eShop.BlazorWebUI.Models;

public class ConfirmChangePhoneNumberModel
{
    public Guid UserId { get; set; }
    public string CurrentPhoneNumberCode { get; set; } = string.Empty;
    public string NewPhoneNumberCode { get; set; } = string.Empty;
}