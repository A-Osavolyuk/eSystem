namespace eShop.Auth.Api.Entities;

public class UserCredentialEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string CredentialId { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public byte[] PublicKey { get; set; } = [];
    public uint SignCount { get; set; }
    public UserEntity User { get; set; } = null!;
}