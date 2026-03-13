using eSecurity.Server.Security.Authorization.Codes;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class CodeEntity : Entity
{
    public Guid Id { get; init; }
    
    public SenderType Sender { get; init; }
    public required string CodeHash { get; init; }

    public CodeState State { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
    public DateTimeOffset? ConsumedAt { get; set; }
    public DateTimeOffset? CancelledAt { get; set; }
    
    public Guid UserId { get; init; }
    public UserEntity? User { get; init; }
}