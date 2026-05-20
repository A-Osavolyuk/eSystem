using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class SessionEntity : Entity
{
    public Guid Id { get; set; }

    public DateTimeOffset? ExpireDate { get; set; }
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    
    public ICollection<OpaqueTokenEntity> OpaqueTokens { get; set; } = [];
    public ICollection<SessionAuthenticationMethodEntity> AuthenticationMethods { get; set; } = [];
    
    public void AddMethods(params IEnumerable<AuthenticationMethodReference> methods)
    {
        foreach (var method in methods)
        {
            if (AuthenticationMethods.All(x => x.MethodReference != method))
            {
                AuthenticationMethods.Add(new SessionAuthenticationMethodEntity()
                {
                    Id = Guid.CreateVersion7(),
                    SessionId = Id,
                    MethodReference = method
                });
            }
        }
    }
}