namespace eShop.Auth.Api.Entities;

public class VerificationEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public CodeResource Resource { get; set; }
    public CodeType Type { get; set; }

    public DateTimeOffset ExpireDate { get; set; }
    public UserEntity User { get; set; } = null!;
}