using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class ClientTokenAuthMethodEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }

    public required string Method { get; set; }

    public ClientEntity Client { get; set; } = null!;
}