using eSecurity.Idp.Security.Authentication.AuthenticationSession;
using eSecurity.Idp.Security.Authentication.Session;
using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class AuthenticationMethodEntity : Entity
{
    public required Guid Id { get; set; }
    public required AuthenticationMethodReference MethodReference { get; set; }
    public required AuthenticationMethodType Type { get; set; }

    public Guid SessionId { get; set; }
    public AuthenticationSessionEntity Session { get; set; } = null!;
}