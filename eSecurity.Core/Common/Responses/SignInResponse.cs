using eSecurity.Core.Security.Authentication.Lockout;

namespace eSecurity.Core.Common.Responses;

public sealed class SignInResponse
{
    public Guid UserId { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
}