using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class ClientGrantTypeEntity : Entity
{
    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;

    public Guid GrantId { get; set; }
    public GrantTypeEntity Grant { get; set; } = null!;
}