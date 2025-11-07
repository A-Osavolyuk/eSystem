using eSecurity.Security.Authorization.Access;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class CodeEntity : Entity, IExpirable
{
    public Guid Id { get; init; } = Guid.CreateVersion7();
    
    public Guid UserId { get; init; }
    public string CodeHash { get; init; } = string.Empty;
    public ActionType Action { get; init; }
    public SenderType Sender { get; init; }
    public PurposeType Purpose { get; init; }
    public DateTimeOffset ExpireDate { get; set; } = DateTime.UtcNow.AddMinutes(10);

    public UserEntity? User { get; init; }
}