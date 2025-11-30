namespace eSystem.Core.Common.Results;

public static class Errors
{
    public static class OAuth
    {
        // ===== OAuth 2.0 Core Errors (RFC 6749) =====
        public const string InvalidRequest = "invalid_request";
        public const string InvalidClient = "invalid_client";
        public const string InvalidGrant = "invalid_grant";
        public const string UnauthorizedClient = "unauthorized_client";
        public const string UnsupportedGrantType = "unsupported_grant_type";
        public const string InvalidScope = "invalid_scope";
        public const string InvalidToken = "invalid_token";

        // Optional OAuth 2.0 errors  
        public const string ServerError = "server_error";
        public const string TemporarilyUnavailable = "temporarily_unavailable"; 

        // ===== OpenID Connect Core Errors (OIDC) =====  
        public const string AccessDenied = "access_denied";
        public const string UnsupportedResponseType = "unsupported_response_type";  
        public const string LoginRequired = "login_required";
        public const string ConsentRequired = "consent_required";
        public const string InteractionRequired = "interaction_required";

        // ===== OpenID Connect Request Object Errors =====  
        public const string InvalidRequestUri = "invalid_request_uri";
        public const string InvalidRequestObject = "invalid_request_object"; 
        public const string RequestNotSupported = "request_not_supported";
        public const string RequestUriNotSupported = "request_uri_not_supported";
        public const string RegistrationNotSupported = "registration_not_supported";
    }
    
    public static class Common
    {
        public const string TooManyFailedLoginAttempts = "too_many_failed_login_attempts";
        public const string FailedLoginAttempt = "failed_login_attempt";
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