namespace eShop.BlazorWebUI.Models;

public class SendTwoFactorTokenModel
{
    public string Email { get; set; }
    public string Provider { get; set; } = string.Empty;
}