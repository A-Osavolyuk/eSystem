namespace eSystem.Domain.Security.Lockout;

/// <summary>
/// Specifies the reason why a user account has been locked out.
/// </summary>
public enum LockoutType
{
    None,
    
    /// <summary>
    /// The account has been locked due to exceeding the allowed number of failed login attempts.
    /// </summary>
    TooManyFailedLoginAttempts,

    /// <summary>
    /// The account has been locked due to detection of suspicious activity, such as abnormal login patterns.
    /// </summary>
    SuspiciousActivity,

    /// <summary>
    /// The account has been manually locked by an administrator.
    /// </summary>
    ManualAdminLockout,

    /// <summary>
    /// The account has been locked due to a violation of the terms of service.
    /// </summary>
    TermsOfServiceViolation,

    /// <summary>
    /// The account has been locked because it is believed to be compromised (e.g., unauthorized access).
    /// </summary>
    AccountCompromised,

    /// <summary>
    /// The account has been temporarily locked for reasons such as maintenance or policy enforcement.
    /// </summary>
    TemporaryLockout,

    /// <summary>
    /// The account has been locked due to a legal request, court order, or regulatory action.
    /// </summary>
    LegalHold,

    /// <summary>
    /// The account has been locked due to payment issues, such as failed or overdue billing.
    /// </summary>
    BillingIssue,

    /// <summary>
    /// The account has been flagged for review after triggering automated security rules.
    /// </summary>
    AutomatedSecurityFlag,

    /// <summary>
    /// The user requested the account to be temporarily locked for personal or security reasons.
    /// </summary>
    UserRequestedLockout,

    /// <summary>
    /// The account has been locked due to inactivity over an extended period.
    /// </summary>
    InactivityTimeout
}

