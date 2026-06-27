using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class UserTwoFactorMethodEntity : Entity
{
    public Guid Id { get; set; }
    
    public bool Preferred { get; set; }
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public Guid MethodId { get; set; }
    public TwoFactorMethodEntity Method { get; set; } = null!;
}