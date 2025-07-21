namespace eShop.Domain.Enums;

/// <summary>
/// Defines the various states or actions related to the verification process.
/// </summary>
public enum CodeType
{
    /// <summary>
    /// Represent verification of the current (existing) resource like email, phone number, etc.
    /// </summary>
    Current = 1,

    /// <summary>
    /// Represents verification of a new resource, such as a newly registered email address or phone number.
    /// </summary>
    New = 2,

    /// <summary>
    /// Represents an action to verify a specific resource or process, ensuring its authenticity or correctness,
    /// such as email or phone verification.
    /// </summary>
    Verify = 3,

    /// <summary>
    /// Represents a verification code specifically generated for resetting a resource,
    /// such as a password or other account-related data.
    /// </summary>
    Reset = 4,
    
    /// <summary>
    /// Represent a recovery code for account recover
    /// </summary>
    Unlock = 5
}