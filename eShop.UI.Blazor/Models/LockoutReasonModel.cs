using eShop.Domain.Enums;

namespace eShop.BlazorWebUI.Models;

public class LockoutReasonModel
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public LockoutType Type { get; set; }
    public LockoutPeriod Period { get; set; }
}