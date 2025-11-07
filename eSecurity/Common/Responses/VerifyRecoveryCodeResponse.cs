using eSecurity.Security.Authentication.Lockout;

namespace eSecurity.Common.Responses;

public class VerifyRecoveryCodeResponse
{
    public Guid UserId { get; set; }
    
    public int FailedLoginAttempts { get; set; }
    public int MaxFailedLoginAttempts { get; set; }

    public bool IsLockedOut { get; set; }
    public LockoutType Type { get; set; }
}