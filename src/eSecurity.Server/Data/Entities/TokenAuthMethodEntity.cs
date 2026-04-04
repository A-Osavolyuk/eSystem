using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.OpenIdConnect.Constants;

namespace eSecurity.Server.Data.Entities;

public sealed class TokenAuthMethodEntity : Entity
{
    public Guid Id { get; set; }
    public required TokenAuthMethod Method { get; set; }
}