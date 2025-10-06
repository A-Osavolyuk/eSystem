namespace eShop.Auth.Api.Entities;

public class UserVerificationMethodEntity : Entity
{
    public Guid UserId { get; set; }
    public Guid MethodId { get; set; }

    public bool IsPrimary { get; set; }

    public UserEntity User { get; set; } = null!;
    public VerificationMethodEntity Method { get; set; } = null!;
}