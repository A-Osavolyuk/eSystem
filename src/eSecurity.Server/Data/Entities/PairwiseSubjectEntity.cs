using eSystem.Core.Data.Entities;

namespace eSecurity.Server.Data.Entities;

public class PairwiseSubjectEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ClientId { get; set; }
    
    public required string SectorIdentifier { get; set; }
    public required string SubjectIdentifier { get; set; }
    
    public UserEntity User { get; set; } = null!;
    public ClientEntity Client { get; set; } = null!;
}