using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Idp.Security.Authorization.Verification;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class VerificationRequestEntity : Entity
{
    public Guid Id { get; set; }
    
    public VerificationMethod Method { get; set; }
    public VerificationStatus Status { get; set; }
    public OperationType Operation { get; set; }

    public string? Target { get; set; }
    public Dictionary<string, string>? Payload { get; set; }
    
    public DateTimeOffset ExpiredAt { get; set; }

    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}
