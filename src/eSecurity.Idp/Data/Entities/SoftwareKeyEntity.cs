using eSecurity.Idp.Security.Credentials.PublicKey.Attestation;
using eSystem.Core.Server.Data.Entities;

namespace eSecurity.Idp.Data.Entities;

public class SoftwareKeyEntity : Entity
{
    public Guid Id { get; set; }
    public Guid AuthenticatorId { get; set; }
    
    public required byte[] CredentialId { get; set; }
    public required string DisplayName { get; set; }
    public required byte[] PublicKey { get; set; }
    public required string Domain { get; set; }
    public required uint SignCount { get; set; }
    public required string Type { get; set; }
    public required AttestationFormatType AttestationFormatType { get; set; }
    public required AttestationTrustType AttestationTrustType { get; set; }
    
    public DateTimeOffset? LastSeenDate { get; set; }
    
    public Guid DeviceId { get; set; }
    public UserDeviceEntity Device { get; set; } = null!;
}