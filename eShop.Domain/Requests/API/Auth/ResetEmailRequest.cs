namespace eShop.Domain.Requests.API.Auth;

public class ResetEmailRequest
{
    public required Guid UserId { get; set; }
    public required string NewEmail { get; set; }
}