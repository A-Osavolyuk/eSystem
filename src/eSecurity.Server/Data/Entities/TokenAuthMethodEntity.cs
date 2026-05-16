using eSystem.Core.Server.Data.Entities;
using eSystem.Core.Server.Security.Authentication.OpenIdConnect;

namespace eSecurity.Server.Data.Entities;

public sealed class TokenAuthMethodEntity : Entity
{
    public Guid Id { get; set; }
    public required TokenAuthMethod Method { get; set; }
}