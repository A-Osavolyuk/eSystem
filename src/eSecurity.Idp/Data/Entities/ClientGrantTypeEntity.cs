using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class ClientGrantTypeEntity : Entity
{
    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;

    public Guid GrantId { get; set; }
    public GrantTypeEntity Grant { get; set; } = null!;
}