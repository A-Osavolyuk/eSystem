namespace eShop.Blazor.Domain.Models;

public class RemovePasskeyModel
{
    public string Code { get; set; } = string.Empty;
    public UserPasskeyDto Passkey { get; set; } = new();
}