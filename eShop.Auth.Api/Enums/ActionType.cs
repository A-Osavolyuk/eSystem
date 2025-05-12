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
    /// Represents full access to all actions.
    /// </summary>
    All = 5
}