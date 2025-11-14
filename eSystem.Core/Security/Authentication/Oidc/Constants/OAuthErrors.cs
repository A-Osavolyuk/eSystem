namespace eSystem.Core.Security.Authentication.Oidc.Constants;

/// <summary>
/// Standard OAuth 2.0 / OpenID Connect error codes
/// Reference: RFC 6749 and OIDC Core 1.0
/// </summary>
public static class OAuthErrors
{
    // Common OAuth 2.0 / Token Endpoint errors
    public const string InvalidRequest = "invalid_request";
    public const string InvalidClient = "invalid_client";
    public const string InvalidGrant = "invalid_grant";
    public const string UnauthorizedClient = "unauthorized_client";
    public const string UnsupportedGrantType = "unsupported_grant_type";
    public const string InvalidScope = "invalid_scope";
    public const string ServerError = "server_error";
    public const string TemporarilyUnavailable = "temporarily_unavailable";

    // Authorization Endpoint / OIDC specific errors
    public const string AccessDenied = "access_denied";
    public const string UnsupportedResponseType = "unsupported_response_type";
    public const string LoginRequired = "login_required";
    public const string ConsentRequired = "consent_required";
    public const string InteractionRequired = "interaction_required";
}