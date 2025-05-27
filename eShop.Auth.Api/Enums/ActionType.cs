namespace eShop.Auth.Api.Enums;

/// <summary>
/// Represents all available permission-based actions within the system.
/// </summary>
public enum ActionType
{
    /// <summary>
    /// Represents no action or an unspecified action type.
    /// </summary>
    None = 0,

    /// <summary>
    /// Represents the action of creating a new resource or entity.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Represents the action of reading or accessing data without making modifications.
    /// </summary>
    Read = 2,

    /// <summary>
    /// Represents the action of modifying or updating existing data.
    /// </summary>
    Update = 3,

    /// <summary>
    /// Represents the action of permanently removing data.
    /// </summary>
    Delete = 4,
    
    /// <summary>
    /// Represents the action of assigning a role, task, or responsibility to a user or resource.
    /// </summary>
    Assign = 20,

    /// <summary>
    /// Represents the action of removing an assigned role, task, or responsibility.
    /// </summary>
    Unassign = 21,

    /// <summary>
    /// Represents the action of granting access or permissions to a user or role.
    /// </summary>
    Grant = 22,

    /// <summary>
    /// Represents the action of revoking access or permissions from a user or role.
    /// </summary>
    Revoke = 23,

    /// <summary>
    /// Represents the action of inviting a user to the system or a specific resource.
    /// </summary>
    Invite = 24,

    /// <summary>
    /// Represents the action of removing a user from the system or a specific group.
    /// </summary>
    Remove = 25,
    
    /// <summary>
    /// Represents the action of temporarily or permanently disabling a user’s ability to access the system or a resource.
    /// </summary>
    Lockout = 26,

    /// <summary>
    /// Represents the action of restoring access for a previously locked out user, re-enabling their ability to interact with the system or resource.
    /// </summary>
    Unlock = 27,
    
    /// <summary>
    /// Represents full access to all actions.
    /// </summary>
    All = 100
}