using eSecurity.Core.Security.Identity;

namespace eSecurity.Core.Common.Requests;

public sealed class ChangeEmailRequest
{
    public required string Subject { get; set; }
    public required string NewEmail { get; set; }
    public required EmailType Type { get; set; }
}