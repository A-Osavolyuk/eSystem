using eSystem.Domain.Security.Authentication.TwoFactor;

namespace eSystem.Domain.DTOs;

public class UserTwoFactorMethod
{
    public required TwoFactorMethod Method { get; set; }
    public required bool Preferred { get; set; }
    public DateTimeOffset? UpdateDate { get; set; }
}