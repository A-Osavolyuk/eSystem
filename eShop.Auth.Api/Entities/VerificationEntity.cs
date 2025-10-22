using eShop.Domain.Security.Verification;

namespace eShop.Auth.Api.Entities;

public class VerificationEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public PurposeType Purpose { get; set; }
    public ActionType Action { get; set; }

    public DateTimeOffset ExpireDate { get; set; }
    public UserEntity User { get; set; } = null!;
}