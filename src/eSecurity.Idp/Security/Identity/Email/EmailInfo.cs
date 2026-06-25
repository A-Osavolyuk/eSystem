using eSecurity.Core.Security.Identity;

namespace eSecurity.Idp.Security.Identity.Email;

public sealed class EmailInfo(EmailType type, bool isVerified, Guid ownerId)
{
    public EmailType Type { get; init; } = type;
    public bool IsVerified { get; init; } = isVerified;
    public Guid OwnerId { get; init; } = ownerId;
}