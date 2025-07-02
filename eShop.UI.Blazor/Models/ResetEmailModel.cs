namespace eShop.BlazorWebUI.Models;

public class ResetEmailModel
{
    public Guid Id { get; set; }
    public string NewEmail { get; set; } = string.Empty;
}