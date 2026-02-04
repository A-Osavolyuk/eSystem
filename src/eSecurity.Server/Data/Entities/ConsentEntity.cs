using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class ConsentEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ClientId { get; set; }

    public UserEntity User { get; set; } = null!;
    public ClientEntity Client { get; set; } = null!;
    public ICollection<GrantedScopeEntity> GrantedScopes { get; set; } = null!;

    public bool HasScopes(IEnumerable<string> scopes, out IEnumerable<string> remainingScopes)
    {
        remainingScopes = GrantedScopes.Select(x => x.ClientScope.Scope.Value).Except(scopes);
        return !remainingScopes.Any();
    }
}