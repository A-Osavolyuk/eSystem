using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Data.Entities;

public sealed class SessionAuthenticationMethodEntity : Entity
{
    public Guid Id { get; set; }

    public AuthenticationMethod Method { get; set; }

    public Guid SessionId { get; set; }
    public SessionEntity Session { get; set; } = null!;
}