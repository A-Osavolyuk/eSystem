using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public sealed class ClientResponseTypeEntity : Entity
{
    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
    
    public Guid ResponseTypeId { get; set; }
    public ResponseTypeEntity ResponseType { get; set; } = null!;
}