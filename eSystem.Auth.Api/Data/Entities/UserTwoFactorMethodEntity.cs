using eSystem.Core.Data.Entities;
using eSystem.Core.Security.Authentication.TwoFactor;

namespace eSystem.Auth.Api.Data.Entities;

public class UserTwoFactorMethodEntity : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public bool Preferred { get; set; }
    public TwoFactorMethod Method { get; set; }
    
    public UserEntity User { get; set; } = null!;
}