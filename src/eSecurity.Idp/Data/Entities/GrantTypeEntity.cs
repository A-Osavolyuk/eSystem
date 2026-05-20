using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class GrantTypeEntity : Entity
{
    public Guid Id { get; set; }

    public required GrantType Grant { get; set; }
}