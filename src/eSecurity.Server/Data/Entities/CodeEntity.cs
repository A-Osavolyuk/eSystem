using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class CodeEntity : Entity, IExpirable
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    
    public SenderType Sender { get; init; }
    public required string CodeHash { get; init; }
    public DateTimeOffset ExpireDate { get; set; } = DateTime.UtcNow.AddMinutes(10);
    
    public Guid UserId { get; init; }
    public UserEntity? User { get; init; }
}