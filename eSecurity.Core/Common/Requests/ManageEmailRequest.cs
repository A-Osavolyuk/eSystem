using eSecurity.Core.Security.Identity;

namespace eSecurity.Core.Common.Requests;

public class ManageEmailRequest
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
    public required EmailType Type { get; set; }
}