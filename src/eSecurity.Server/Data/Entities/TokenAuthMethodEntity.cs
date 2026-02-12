using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class TokenAuthMethodEntity : Entity
{
    public Guid Id { get; set; }
    public required string Method { get; set; }
}