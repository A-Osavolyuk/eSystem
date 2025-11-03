using eSystem.Core.Data.Entities;

namespace eSecurity.Data.Entities;

public class PasskeyEntity : Entity
{
    public Guid Id { get; set; }
    public Guid AuthenticatorId { get; set; }
    public Guid DeviceId { get; set; }
    
    public string CredentialId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public byte[] PublicKey { get; set; } = [];
    public string Domain { get; set; } = string.Empty;
    public uint SignCount { get; set; }
    public string Type { get; set; } = string.Empty;
    
    public DateTimeOffset? LastSeenDate { get; set; }
    
    public UserDeviceEntity Device { get; set; } = null!;
}