namespace eAccount.Blazor.Server.Domain.Models;

public class AddPasswordModel
{
    public Guid UserId { get; set; }
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}