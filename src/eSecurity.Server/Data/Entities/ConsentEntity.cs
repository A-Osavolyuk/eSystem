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
    
    public bool HasScope(string scope) => GrantedScopes.Any(x => x.Scope.Name == scope);
    public bool HasScopes(IEnumerable<string> scopes) 
        => scopes.All(scope => GrantedScopes.Any(grantedScope => grantedScope.Scope.Name == scope));
}