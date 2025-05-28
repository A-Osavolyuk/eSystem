namespace eShop.Domain.Enums;

/// <summary>
/// Specifies the reason why a user account has been locked out.
/// </summary>
public enum LockoutReason
{
    /// <summary>
    /// The user is not locked out.
    /// </summary>
    None = 0,

    /// <summary>
    /// The account has been locked due to exceeding the allowed number of failed login attempts.
    /// </summary>
    TooManyFailedLoginAttempts = 1,

    /// <summary>
    /// The account has been locked due to detection of suspicious activity, such as abnormal login patterns.
    /// </summary>
    SuspiciousActivity = 2,

    /// <summary>
    /// The account has been manually locked by an administrator.
    /// </summary>
    ManualAdminLockout = 3,

    /// <summary>
    /// The account has been locked due to a violation of the terms of service.
    /// </summary>
    TermsOfServiceViolation = 4,

    /// <summary>
    /// The account has been locked because it is believed to be compromised (e.g., unauthorized access).
    /// </summary>
    AccountCompromised = 5,

    /// <summary>
    /// The account has been temporarily locked for reasons such as maintenance or policy enforcement.
    /// </summary>
    TemporaryLockout = 6,

    /// <summary>
    /// The account has been locked due to a legal request, court order, or regulatory action.
    /// </summary>
    LegalHold = 7,

    /// <summary>
    /// The account has been locked due to payment issues, such as failed or overdue billing.
    /// </summary>
    BillingIssue = 8,

    /// <summary>
    /// The account has been flagged for review after triggering automated security rules.
    /// </summary>
    AutomatedSecurityFlag = 9,

    /// <summary>
    /// The user requested the account to be temporarily locked for personal or security reasons.
    /// </summary>
    UserRequestedLockout = 10,

    /// <summary>
    /// The account has been locked due to inactivity over an extended period.
    /// </summary>
    InactivityTimeout = 11
}

