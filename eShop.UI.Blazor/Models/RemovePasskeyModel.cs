using eShop.Domain.DTOs;

namespace eShop.BlazorWebUI.Models;

public class RemovePasskeyModel
{
    public string Code { get; set; } = string.Empty;
    public UserPasskeyDto Passkey { get; set; } = new();
}