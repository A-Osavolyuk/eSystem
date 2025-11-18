namespace eSystem.Core.Common.Results;

public partial class Errors
{
    /// <summary>
    /// Standard OAuth 2.0 / OpenID Connect error codes
    /// Reference: RFC 6749 and OIDC Core 1.0
    /// </summary>
    public static class OAuth
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

    public static class Common
    {
        public const string TooManyFailedLoginAttempts = "too_many_failed_login_attempts";
        public const string FailedLoginAttempt = "failed_login_attempt";
        public const string InvalidCode = "invalid_code";
        public const string InvalidPayloadType = "invalid_payload_type";
        public const string InvalidDevice = "invalid_device";
        public const string UntrustedDevice = "untrusted_device";
        public const string UnverifiedEmail = "unverified_email";
        public const string AccountLockedOut = "account_locked_out";
        public const string BlockedDevice = "blocked_device";
        public const string BadRequest = "bad_request";
        public const string Unauthorized = "unauthotized";
        public const string Forbidden = "forbidden";
        public const string NotFound = "not_found";
        public const string TooManyRequests = "too_many_requests";
        public const string InternalServerError = "internal_server_error";
    }
}