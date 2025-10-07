namespace eShop.Blazor.Server.Domain.Types;

public class ConfirmationContext
{
    public required CodeResource Resource { get; set; }
    public required CodeType Type { get; set; }
}