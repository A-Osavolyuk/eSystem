using eSecurity.Core.Security.Identity;

namespace eSecurity.Core.Common.Requests;

public sealed class ManageEmailRequest
{
    public required string Subject { get; set; }
    public required string Email { get; set; }
    public required EmailType Type { get; set; }
}