using eShop.Domain.Enums;

namespace eShop.BlazorWebUI.Models;

public class OAuthFallbackModel
{
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
}