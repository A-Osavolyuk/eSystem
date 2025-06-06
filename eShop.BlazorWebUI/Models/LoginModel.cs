namespace eShop.BlazorWebUI.Models;

public class LoginModel
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public int FailedLoginAttempts { get; set; }
}