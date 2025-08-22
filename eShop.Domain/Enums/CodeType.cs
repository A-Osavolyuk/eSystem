namespace eShop.Domain.Enums;

/// <summary>
/// Defines the various states or actions related to the verification process.
/// </summary>
public enum CodeType
{
    /// <summary>
    /// Verification of an existing resource, such as the current email or phone number.
    /// </summary>
    Current = 1,

    /// <summary>
    /// Verification of a newly added resource, such as a new email address or phone number.
    /// </summary>
    New = 2,

    /// <summary>
    /// Generic verification of a resource or process, e.g. email or phone confirmation.
    /// </summary>
    Verify = 3,

    /// <summary>
    /// Verification for resetting a resource, such as a password or other account-related data.
    /// </summary>
    Reset = 4,

    /// <summary>
    /// Recovery code used to regain access to an account (e.g., after losing credentials).
    /// </summary>
    Unlock = 5,

    /// <summary>
    /// Trusting a user device, marking it as recognized for future sign-ins.
    /// </summary>
    Trust = 6,

    /// <summary>
    /// Blocking a user device, preventing it from being used for sign-in.
    /// </summary>
    Block = 7,

    /// <summary>
    /// Unblocking a previously blocked device, allowing it to be used again.
    /// </summary>
    Unblock = 8,

    /// <summary>
    /// Disconnecting a linked external account (e.g., Google, Facebook, etc.).
    /// </summary>
    Disconnect = 9,

    /// <summary>
    /// Allowing sign-in via a linked external account.
    /// </summary>
    Allow = 10,

    /// <summary>
    /// Disallowing sign-in via a linked external account (opposite of <see cref="Allow"/>).
    /// </summary>
    Disallow = 11,
    
    /// <summary>
    /// Verification of remove action.
    /// </summary>
    Remove = 12
}