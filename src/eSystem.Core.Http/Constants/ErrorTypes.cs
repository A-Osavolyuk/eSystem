namespace eSystem.Core.Http.Constants;

public static class ErrorTypes
{
    public static class OAuth
    {
        public const string InvalidRequest = "invalid_request";
        public const string InvalidClient = "invalid_client";
        public const string InvalidGrant = "invalid_grant";
        public const string UnauthorizedClient = "unauthorized_client";
        public const string UnsupportedGrantType = "unsupported_grant_type";
        public const string InvalidScope = "invalid_scope";
        public const string InvalidToken = "invalid_token";
        public const string InvalidTarget = "invalid_target";
        public const string ServerError = "server_error";
        public const string TemporarilyUnavailable = "temporarily_unavailable"; 
        public const string AccessDenied = "access_denied";
        public const string UnsupportedResponseType = "unsupported_response_type";
        public const string UnsupportedTokenType = "unsupported_token_type";
        public const string LoginRequired = "login_required";
        public const string ConsentRequired = "consent_required";
        public const string InteractionRequired = "interaction_required";
        public const string InsufficientScope = "insufficient_scope";
        public const string SlowDown = "slow_down";
        public const string ExpiredToken = "expired_token";
        public const string AuthorizationPending = "authorization_pending";
        public const string InvalidRequestUri = "invalid_request_uri";
        public const string InvalidRequestObject = "invalid_request_object"; 
        public const string RequestNotSupported = "request_not_supported";
        public const string RequestUriNotSupported = "request_uri_not_supported";
        public const string RegistrationNotSupported = "registration_not_supported";
        public const string MissingUserCode = "missing_user_code";
        public const string InvalidUserCode = "invalid_user_code";
        public const string InvalidBindingMessage = "invalid_binding_message";
        public const string UnknownUserId = "unknown_user_id";
        public const string ExpiredLoginTokenHint = "expired_login_hint_token";
    }
    
    public static class Common
    {
        public const string InvalidPayloadType = "invalid_payload_type";
        public const string InvalidPassword = "invalid_password";
        public const string InvalidDevice = "invalid_device";
        public const string InvalidEmail = "invalid_email";
        public const string InvalidPhone = "invalid_phone";
        public const string InvalidLockoutState = "invalid_lockout_state";
        public const string InvalidCredentials = "invalid_credentials";
        public const string InvalidSession = "invalid_session";
        public const string ExpiredAuthenticationSession = "expired_authentication_session";
        public const string InvalidRp = "invalid_rp";
        public const string InvalidChallenge = "invalid_challenge";

        public const string EmailTaken = "email_taken";
        public const string PhoneTaken = "phone_taken";
        public const string UsernameTaken = "username_taken";
        
        public const string TooManyFailedLoginAttempts = "too_many_failed_login_attempts";
        public const string FailedLoginAttempt = "failed_login_attempt";
        public const string UntrustedDevice = "untrusted_device";
        public const string UnverifiedEmail = "unverified_email";
        public const string AccountLockedOut = "account_locked_out";
        public const string BlockedDevice = "blocked_device";
        public const string MaxEmailsCount = "max_emails_count";
        public const string LinkedAccountConnected = "linked_account_connected";
        
        public const string BadRequest = "bad_request";
        public const string MethodNotAllowed = "method_not_allowed";
        public const string UnsupportedMediaType = "unsupported_media_type";
        public const string Unauthorized = "unauthotized";
        public const string Forbidden = "forbidden";
        public const string NotFound = "not_found";
        public const string TooManyRequests = "too_many_requests";
        public const string InternalServerError = "internal_server_error";
    }
}