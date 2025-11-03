using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Data.Entities;

public class UserVerificationMethodEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool Preferred { get; set; }
    public VerificationMethod Method { get; set; }

    public UserEntity User { get; set; } = null!;
}