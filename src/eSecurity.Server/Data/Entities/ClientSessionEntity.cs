using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class ClientSessionEntity : Entity
{
    public Guid ClientId { get; set; }
    public Guid SessionId { get; set; }

    public ClientEntity Client { get; set; } = null!;
    public SessionEntity Session { get; set; } = null!;
}