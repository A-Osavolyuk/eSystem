using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class UserClientEntity : Entity
{
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    
    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
}