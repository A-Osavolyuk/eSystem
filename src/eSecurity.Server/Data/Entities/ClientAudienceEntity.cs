using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class ClientAudienceEntity : Entity
{
    public required Guid Id { get; set; }
    public required string Audience { get; set; }
    
    public required Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
}