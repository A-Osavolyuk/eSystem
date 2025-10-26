using eSystem.Core.Data.Entities;

namespace eSystem.Auth.Api.Entities;

public class ClientRedirectUriEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public required string RedirectUri { get; set; }

    public ClientEntity Client { get; set; } = null!;
}