namespace eAccount.Blazor.Server.Domain.Models;

public class ResetPhoneNumberModel
{
    public Guid Id { get; set; }
    public string NewPhoneNumber { get; set; } = string.Empty;
}