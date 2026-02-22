using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class VerificationRequestEntity : Entity
{
    public Guid Id { get; set; }

    public ActionType Action { get; set; }
    public PurposeType Purpose { get; set; }
    public VerificationMethod Method { get; set; }
    public VerificationStatus Status { get; set; }
    
    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public DateTimeOffset? ConsumedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}
