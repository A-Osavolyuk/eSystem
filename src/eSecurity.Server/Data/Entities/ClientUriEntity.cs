using eSecurity.Server.Security.Authentication.Oidc.Client;
using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class ClientUriEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }

    public required UriType Type { get; set; }
    public required string Uri { get; set; }

    public ClientEntity Client { get; set; } = null!;
}