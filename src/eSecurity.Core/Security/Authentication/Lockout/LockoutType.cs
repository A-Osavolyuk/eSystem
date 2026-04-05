using eSystem.Core.Enums;

namespace eSecurity.Core.Security.Authentication.Lockout;

[JsonConverter(typeof(JsonEnumValueStringConverter<LockoutPeriod>))]
public enum LockoutType
{
    [EnumValue("none")]
    None,
    
    [EnumValue("too_many_failed_login_attempts")]
    TooManyFailedLoginAttempts,
    
    [EnumValue("suspicious_activity")]
    SuspiciousActivity,
    
    [EnumValue("manual_admin_lockout")]
    ManualAdminLockout,
    
    [EnumValue("terms_of_service_violation")]
    TermsOfServiceViolation,
    
    [EnumValue("account_compromised")]
    AccountCompromised,
    
    [EnumValue("temporary_lockout")]
    TemporaryLockout,
    
    [EnumValue("legal_hold")]
    LegalHold,
    
    [EnumValue("billing_issue")]
    BillingIssue,
    
    [EnumValue("automated_security_flag")]
    AutomatedSecurityFlag,
    
    [EnumValue("user_requested_lockout")]
    UserRequestedLockout,
    
    [EnumValue("inactivity_timeout")]
    InactivityTimeout
}

