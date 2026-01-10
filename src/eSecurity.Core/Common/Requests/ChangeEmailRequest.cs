using eSecurity.Core.Security.Identity;

namespace eSecurity.Core.Common.Requests;

public class ChangeEmailRequest
{
    public required Guid UserId { get; set; }
    public required string NewEmail { get; set; }
    public required EmailType Type { get; set; }
}