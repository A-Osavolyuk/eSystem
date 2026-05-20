using eSystem.Core.Security.Authentication.OpenIdConnect;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class TokenAuthMethodEntity : Entity
{
    public Guid Id { get; set; }
    public required TokenAuthMethod Method { get; set; }
}