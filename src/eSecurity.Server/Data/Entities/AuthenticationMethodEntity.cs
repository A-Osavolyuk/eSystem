using eSecurity.Server.Security.Authentication.Session;
using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Data.Entities;

public sealed class AuthenticationMethodEntity : Entity
{
    public required Guid Id { get; set; }
    public required AuthenticationMethodReference MethodReference { get; set; }
    public required AuthenticationMethodType Type { get; set; }

    public Guid SessionId { get; set; }
    public AuthenticationSessionEntity Session { get; set; } = null!;
}