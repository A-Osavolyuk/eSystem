using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Common.Validation;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class UserEmailEntity() : Entity
{
    public UserEmailEntity(Guid userId, string email, EmailType type) : this()
    {
        Id = Guid.CreateVersion7();
        UserId = userId;
        Email = email;
        NormalizedEmail = Normalizer.Normalize(email);
        Type = type;
    }
    
    public Guid Id { get; init; }

    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public EmailType Type { get; set; }
    public bool IsVerified { get; set; }

    public DateTimeOffset? VerifiedAt { get; set; }

    public Guid UserId { get; init; }
    public UserEntity User { get; init; } = null!;
}