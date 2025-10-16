namespace eShop.Auth.Api.Entities;

public class UserPasskeyEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AuthenticatorId { get; set; }
    public string CredentialId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public byte[] PublicKey { get; set; } = [];
    public string Domain { get; set; } = string.Empty;
    public uint SignCount { get; set; }
    public string Type { get; set; } = string.Empty;
    
    public DateTimeOffset? LastSeenDate { get; set; }
    
    public UserEntity User { get; set; } = null!;
}