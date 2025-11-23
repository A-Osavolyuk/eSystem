using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class ClientFrontChannelLogoutUriEntity : Entity
{
    public required Guid Id { get; set; }
    public required Guid ClientId { get; set; }
    public required string Uri { get; set; }

    public ClientEntity Client { get; set; } = null!;
}