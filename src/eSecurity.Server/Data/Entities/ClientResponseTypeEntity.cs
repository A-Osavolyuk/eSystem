using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class ClientResponseTypeEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }

    public required string ResponseType { get; set; }

    public ClientEntity Client { get; set; } = null!;
}