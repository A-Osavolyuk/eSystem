namespace eShop.Domain.Enums;

/// <summary>
/// Defines the various states or actions related to the verification process.
/// </summary>
public enum ActionType
{
    /// <summary>
    /// Verification of an existing resource, such as the current email or phone number.
    /// </summary>
    Current,

    /// <summary>
    /// Verification of a newly added resource, such as a new email address or phone number.
    /// </summary>
    New,

    /// <summary>
    /// Generic verification of a resource or process, e.g. email or phone confirmation.
    /// </summary>
    Verify,

    /// <summary>
    /// Verification for resetting a resource, such as a password or other account-related data.
    /// </summary>
    Reset,

    /// <summary>
    /// Recovery code used to regain access to an account (e.g., after losing credentials).
    /// </summary>
    Unlock,

    /// <summary>
    /// Trusting a user device, marking it as recognized for future sign-ins.
    /// </summary>
    Trust,

    /// <summary>
    /// Blocking a user device, preventing it from being used for sign-in.
    /// </summary>
    Block,

    /// <summary>
    /// Unblocking a previously blocked device, allowing it to be used again.
    /// </summary>
    Unblock,

    /// <summary>
    /// Disconnecting a linked external account (e.g., Google, Facebook, etc.).
    /// </summary>
    Disconnect,

    /// <summary>
    /// Allowing sign-in via a linked external account.
    /// </summary>
    Allow,

    /// <summary>
    /// Disallowing sign-in via a linked external account (opposite of <see cref="Allow"/>).
    /// </summary>
    Disallow,
    
    /// <summary>
    /// Verification of remove action.
    /// </summary>
    Remove,
    
    /// <summary>
    /// Represents sign-in with 2FA (Two-Factor Authentication)
    /// </summary>
    SignIn,
    
    /// <summary>
    /// Represents subscription to (Two-Factor Authentication) 2FA provider
    /// </summary>
    Subscribe,
    
    /// <summary>
    /// Represents unsubscription to (Two-Factor Authentication) 2FA provider
    /// </summary>
    Unsubscribe,
    
    /// <summary>
    /// Represents user's account access recovery
    /// </summary>
    Recover,
    
    /// <summary>
    /// Represents user's login method enable
    /// </summary>
    Enable,
    
    /// <summary>
    /// Represents user's login method disable
    /// </summary>
    Disable,
    
    /// <summary>
    /// Represents user's emails role management
    /// </summary>
    Manage,
    
    /// <summary>
    /// Represents user's passkey creation
    /// </summary>
    Create,
}