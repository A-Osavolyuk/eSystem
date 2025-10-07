namespace eShop.Auth.Api.Entities;

public class UserVerificationMethodEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool IsPrimary { get; set; }
    public VerificationMethod Method { get; set; }

    public UserEntity User { get; set; } = null!;
}