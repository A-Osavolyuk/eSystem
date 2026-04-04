using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Data.Entities;

public class SessionEntity : Entity
{
    public Guid Id { get; set; }

    public DateTimeOffset? ExpireDate { get; set; }
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    
    public ICollection<OpaqueTokenEntity> OpaqueTokens { get; set; } = null!;
    public ICollection<SessionAuthenticationMethodEntity> AuthenticationMethods { get; set; } = null!;
    
    public void AddMethods(params IEnumerable<AuthenticationMethod> methods)
    {
        foreach (var method in methods)
        {
            if (AuthenticationMethods.All(x => x.Method != method))
            {
                AuthenticationMethods.Add(new SessionAuthenticationMethodEntity()
                {
                    Id = Guid.CreateVersion7(),
                    SessionId = Id,
                    Method = method
                });
            }
        }
    }
}