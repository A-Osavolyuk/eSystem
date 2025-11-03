using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class ClientRedirectUriEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public required string Uri { get; set; }

    public ClientEntity Client { get; set; } = null!;
}