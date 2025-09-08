namespace eShop.Domain.Requests.API.Auth;

public class AddEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; } = string.Empty;
    public required bool IsPrimary { get; set; }
    public required bool IsRecovery { get; set; }
}