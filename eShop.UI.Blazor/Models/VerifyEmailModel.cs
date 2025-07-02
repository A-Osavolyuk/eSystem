namespace eShop.BlazorWebUI.Models;

public class VerifyEmailModel
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}