using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class PairwiseSubjectEntity : Entity
{
    public Guid Id { get; set; }
    
    public required string SectorIdentifier { get; set; }
    public required string Subject { get; set; }
    
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
    
    public Guid ClientId { get; set; }
    public ClientEntity Client { get; set; } = null!;
}