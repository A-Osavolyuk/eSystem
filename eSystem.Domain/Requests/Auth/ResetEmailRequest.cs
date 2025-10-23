namespace eSystem.Domain.Requests.Auth;

public class ResetEmailRequest
{
    public required Guid UserId { get; set; }
    public required string NewEmail { get; set; }
}