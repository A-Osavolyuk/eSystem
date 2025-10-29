using eSystem.Core.Security.Identity.Email;

namespace eSystem.Core.Requests.Auth;

public record ChangeEmailRequest
{
    public required Guid UserId { get; set; }
    public required EmailType Type { get; set; }
    public required string NewEmail { get; set; }
}