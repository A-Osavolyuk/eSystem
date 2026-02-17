namespace eSecurity.Client.Common.Models;

public class AddPasswordModel
{
    public string Subject { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}