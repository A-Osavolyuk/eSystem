namespace eShop.BlazorWebUI.Models;

public class SendTwoFactorTokenModel
{
    public string Email { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}