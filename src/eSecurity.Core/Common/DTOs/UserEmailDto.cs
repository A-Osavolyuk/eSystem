using eSecurity.Core.Security.Identity;

namespace eSecurity.Core.Common.DTOs;

public class UserEmailDto
{
    public Guid Id { get; set; }
    
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    
    public EmailType Type { get; set; }
    public bool IsVerified { get; set; }

    public DateTimeOffset? VerifiedDate { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}