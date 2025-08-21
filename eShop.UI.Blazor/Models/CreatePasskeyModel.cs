namespace eShop.BlazorWebUI.Models;

public class CreatePasskeyModel
{
    public Guid UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
}