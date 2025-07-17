using eShop.Domain.Abstraction.Data.Seeding;

namespace eShop.Auth.Api.Data.Seeding;

public class LockoutReasonSeed : Seed<LockoutReasonEntity>
{
    public override List<LockoutReasonEntity> Get()
    {
        return
        [
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.TooManyFailedLoginAttempts,
                Period = LockoutPeriod.Permanent,
                Code = "TOO_MANY_FAILED_LOGIN_ATTEMPTS",
                Name = "Too many failed login attempts",
                Description = "The account was locked due to multiple unsuccessful login attempts exceeding the allowed limit (5).",
            },
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.ManualAdminLockout,
                Period = LockoutPeriod.Custom,
                Code = "MANUAL_ADMIN_LOCKOUT",
                Name = "Manual admin lockout",
                Description = "The account was manually locked by an administrator for security or policy reasons.",
            },
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.TermsOfServiceViolation,
                Period = LockoutPeriod.Month,
                Code = "TERMS_OF_SERVICE_VIOLATION",
                Name = "Terms of Service Violation",
                Description = "The account was locked due to a violation of the platform’s terms of service.",
            },
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.AccountCompromised,
                Period = LockoutPeriod.Permanent,
                Code = "ACCOUNT_COMPROMISED",
                Name = "Account compromised",
                Description = "The account was locked because it was suspected to be compromised or accessed by unauthorized parties.",
            },
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.TemporaryLockout,
                Period = LockoutPeriod.Day,
                Code = "TEMPORARY_LOCKOUT",
                Name = "Temporary lockout",
                Description = "The account is temporarily locked due to unusual activity or security concerns.",
            },
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.LegalHold,
                Period = LockoutPeriod.Custom,
                Code = "LEGAL_HOLD",
                Name = "Legal hold",
                Description = "The account is under a legal hold due to an ongoing investigation or legal matter.",
            },
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.BillingIssue,
                Period = LockoutPeriod.Day,
                Code = "BILLING_ISSUE",
                Name = "Billing issue",
                Description = "The account was locked due to unresolved billing or payment issues.",
            },
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.AutomatedSecurityFlag,
                Period = LockoutPeriod.Day,
                Code = "AUTOMATED_SECURITY_FLAG",
                Name = "Automated security flag",
                Description = "The account was automatically locked by the system due to suspicious or flagged behavior.",
            },
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.UserRequestedLockout,
                Period = LockoutPeriod.Custom,
                Code = "USER_REQUESTED_LOCKOUT",
                Name = "User requested lockout",
                Description = "The account was locked at the request of the user for personal or security reasons.",
            },
            new LockoutReasonEntity()
            {
                Id = Guid.CreateVersion7(),
                Type = LockoutType.InactivityTimeout,
                Period = LockoutPeriod.Permanent,
                Code = "INACTIVITY_TIMEOUT",
                Name = "Inactivity timeout",
                Description = "The account was locked due to extended period of inactivity.",
            },
        ];
    }
}