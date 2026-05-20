using eSecurity.Idp.Security.Authentication.OpenIdConnect.Client;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class ClientUriEntity : Entity
{
    public Guid Id { get; set; }

    public required UriType Type { get; set; }
    public required string Uri { get; set; }

    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
}