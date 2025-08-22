namespace eShop.Domain.Enums;

/// <summary>
/// Defines the types of resources that can be associated with a verification code
/// </summary>
public enum CodeResource
{
    /// <summary>
    /// The user's primary email address.
    /// </summary>
    Email,

    /// <summary>
    /// The user's recovery or backup email address.
    /// </summary>
    RecoveryEmail,

    /// <summary>
    /// The user's phone number.
    /// </summary>
    PhoneNumber,

    /// <summary>
    /// The entire user account.
    /// </summary>
    Account,

    /// <summary>
    /// The user's account password.
    /// </summary>
    Password,

    /// <summary>
    /// A specific user device (e.g., computer, phone, tablet).
    /// </summary>
    Device,

    /// <summary>
    /// An external linked account (e.g., Google, Facebook, GitHub).
    /// </summary>
    LinkedAccount,
    
    /// <summary>
    /// User's passkey
    /// </summary>
    Passkey
}