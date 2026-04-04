using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Server.Data.Entities;

public sealed class GrantTypeEntity : Entity
{
    public Guid Id { get; set; }

    public required GrantType Grant { get; set; }
}