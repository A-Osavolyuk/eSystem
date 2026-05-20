using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public sealed class OpaqueTokenAudienceEntity : Entity
{
    public Guid Id { get; set; }
    
    public Guid TokenId { get; set; }
    public OpaqueTokenEntity Token { get; set; } = null!;

    public Guid AudienceId { get; set; }
    public ClientAudienceEntity Audience { get; set; } = null!;
}