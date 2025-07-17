namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILockoutManager), ServiceLifetime.Scoped)]
public sealed class LockoutManager(AuthDbContext context) : ILockoutManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<LockoutStateEntity> FindAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutState.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);
        return entity;
    }

    public async ValueTask<Result> LockoutAsync(UserEntity userEntity, LockoutReason reason, string description,
        LockoutPeriod period, DateTimeOffset? endDate = null, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutState.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);

        var lockoutEndDate = period switch
        {
            LockoutPeriod.Day => DateTimeOffset.UtcNow.AddDays(1),
            LockoutPeriod.Week => DateTimeOffset.UtcNow.AddDays(7),
            LockoutPeriod.Month => DateTimeOffset.UtcNow.AddMonths(1),
            LockoutPeriod.Quarter => DateTimeOffset.UtcNow.AddMonths(3),
            LockoutPeriod.Year => DateTimeOffset.UtcNow.AddYears(1),
            LockoutPeriod.Permanent or LockoutPeriod.None => null,
            LockoutPeriod.Custom => endDate,
            _ => throw new NotSupportedException("Not supported period")
        };

        var reasonName = reason switch
        {
            LockoutReason.AccountCompromised => "Account is compromised",
            LockoutReason.TooManyFailedLoginAttempts => "Too many failed login attempts",
            LockoutReason.AutomatedSecurityFlag => "Automated security flag",
            LockoutReason.BillingIssue => "Billing issue",
            LockoutReason.InactivityTimeout => "Inactivity timeout",
            LockoutReason.LegalHold => "Legal hold",
            LockoutReason.SuspiciousActivity => "Suspicious activity",
            LockoutReason.ManualAdminLockout => "Manual admin lockout",
            LockoutReason.TemporaryLockout => "Temporary lockout",
            LockoutReason.TermsOfServiceViolation => "Terms of service violation",
            LockoutReason.UserRequestedLockout => "User requested lockout",
            _  => null
        };

        var code = reason switch
        {
            LockoutReason.AccountCompromised => "ACCOUNT_COMPROMISED",
            LockoutReason.TooManyFailedLoginAttempts => "TOO_MANY_FAILED_LOGIN_ATTEMPTS",
            LockoutReason.AutomatedSecurityFlag => "AUTOMATED_SECURITY_FLAG",
            LockoutReason.BillingIssue => "BILLING_ISSUE",
            LockoutReason.InactivityTimeout => "INACTIVITY_TIMEOUT",
            LockoutReason.LegalHold => "LEGAL_HOLD",
            LockoutReason.SuspiciousActivity => "SUSPICIOUS_ACTIVITY",
            LockoutReason.ManualAdminLockout => "MANUAL_ADMIN_LOCKOUT",
            LockoutReason.TemporaryLockout => "TEMPORARY_LOCKOUT",
            LockoutReason.TermsOfServiceViolation => "TERMS_OF_SERVICE_VIOLATION",
            LockoutReason.UserRequestedLockout => "USER_REQUESTED_LOCKOUT",
            _ => null
        };
        
        entity.Enabled = true;
        entity.Reason = reasonName;
        entity.Code = code;
        entity.Description = description;
        entity.UpdateDate = DateTime.UtcNow;
        entity.Permanent = period is LockoutPeriod.Permanent;
        entity.EndDate = lockoutEndDate;

        context.LockoutState.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnlockAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var entity = await context.LockoutState.FirstAsync(x => x.UserId == userEntity.Id, cancellationToken);

        entity.Enabled = false;
        entity.Reason = string.Empty;
        entity.Code = string.Empty;
        entity.Description = string.Empty;
        entity.EndDate = null;
        entity.UpdateDate = DateTime.UtcNow;

        context.LockoutState.Update(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}