namespace eShop.Auth.Api.Entities;

public class UserCredentialEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string CredentialId { get; set; } = string.Empty;
    public byte[] PublicKey { get; set; } = [];
    public uint SignCount { get; set; }
    public string RpId { get; set; } = string.Empty;
    public Guid AaGuid { get; set; }
    public byte Flags { get; set; }
    public string AttestationType { get; set; } = string.Empty;

    public UserEntity User { get; set; } = null!;
}