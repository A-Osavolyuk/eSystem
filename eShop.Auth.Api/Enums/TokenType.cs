namespace eShop.Auth.Api.Enums;

/// <summary>
/// Represents the type of token used in authentication and authorization.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Represents an access token, typically used to access protected resources.
    /// </summary>
    Access,

    /// <summary>
    /// Represents a refresh token, used to obtain a new access token when the current one expires.
    /// </summary>
    Refresh
}
