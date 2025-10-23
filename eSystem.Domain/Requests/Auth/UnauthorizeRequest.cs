namespace eSystem.Domain.Requests.Auth;

public class UnauthorizeRequest
{
    public required Guid UserId { get; set; }
    public required string RefreshToken { get; set; }
}