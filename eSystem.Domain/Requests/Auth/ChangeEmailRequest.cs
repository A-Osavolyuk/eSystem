using eSystem.Domain.Enums;

namespace eSystem.Domain.Requests.Auth;

public record ChangeEmailRequest
{
    public required Guid UserId { get; set; }
    public required EmailType Type { get; set; }
    public required string NewEmail { get; set; }
}