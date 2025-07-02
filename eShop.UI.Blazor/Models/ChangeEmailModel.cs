namespace eShop.BlazorWebUI.Models;

public class ChangeEmailModel
{
    public Guid UserId { get; set; }
    public string NewEmail { get; set; } = string.Empty;
}