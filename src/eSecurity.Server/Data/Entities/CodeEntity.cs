using eSystem.Core.Common.Messaging;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class CodeEntity : Entity
{
    public Guid Id { get; init; }
    
    public SenderType Sender { get; init; }
    public required string CodeHash { get; init; }
    public DateTimeOffset ExpireDate { get; set; } = DateTime.UtcNow.AddMinutes(10);
    
    public Guid UserId { get; init; }
    public UserEntity? User { get; init; }
}