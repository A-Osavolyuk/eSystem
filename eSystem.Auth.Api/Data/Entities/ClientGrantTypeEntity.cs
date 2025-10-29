using eSystem.Core.Data.Entities;

namespace eSystem.Auth.Api.Data.Entities;

public class ClientGrantTypeEntity : Entity
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public required string Type { get; set; }

    public ClientEntity Client { get; set; } = null!;
}