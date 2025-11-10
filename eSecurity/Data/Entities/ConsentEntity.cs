using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class ConsentEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ClientId { get; set; }

    public UserEntity User { get; set; } = null!;
    public ClientEntity Client { get; set; } = null!;
    public ICollection<GrantedScopeEntity> GrantedScopes { get; set; } = null!;
}