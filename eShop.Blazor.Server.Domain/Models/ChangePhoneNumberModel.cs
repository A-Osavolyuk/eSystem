namespace eShop.Blazor.Server.Domain.Models;

public class ChangePhoneNumberModel
{
    public Guid UserId { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
}