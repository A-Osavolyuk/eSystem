using System.Text.Json.Serialization;
using eSystem.Core.Enums;

namespace eSystem.Core.Primitives;

[JsonConverter(typeof(JsonEnumValueConverter<ErrorCode>))]
public enum ErrorCode
{
    [EnumValue("invalid_request")]
    InvalidRequest,
    
    [EnumValue("invalid_client")]
    InvalidClient,
    
    [EnumValue("invalid_grant")]
    InvalidGrant,
    
    [EnumValue("unauthorized_client")]
    UnauthorizedClient,
    
    [EnumValue("unsupported_grant_type")]
    UnsupportedGrantType,
    
    [EnumValue("invalid_scope")]
    InvalidScope,
    
    [EnumValue("invalid_token")]
    InvalidToken,
    
    [EnumValue("invalid_target")]
    InvalidTarget,
    
    [EnumValue("server_error")]
    ServerError,
    
    [EnumValue("temporarily_unavailable")]
    TemporarilyUnavailable,
    
    [EnumValue("access_denied")]
    AccessDenied,
    
    [EnumValue("unsupported_response_type")]
    UnsupportedResponseType,
    
    [EnumValue("unsupported_token_type")]
    UnsupportedTokenType,
    
    [EnumValue("login_required")]
    LoginRequired,
    
    [EnumValue("consent_required")]
    ConsentRequired,
    
    [EnumValue("interaction_required")]
    InteractionRequired,
    
    [EnumValue("insufficient_scope")]
    InsufficientScope,
    
    [EnumValue("slow_down")]
    SlowDown,
    
    [EnumValue("expired_token")]
    ExpiredToken,
    
    [EnumValue("authorization_pending")]
    AuthorizationPending,
    
    [EnumValue("invalid_request_uri")]
    InvalidRequestUri,
    
    [EnumValue("invalid_request_object")]
    InvalidRequestObject,
    
    [EnumValue("request_not_supported")]
    RequestNotSupported,
    
    [EnumValue("request_uri_not_supported")]
    RequestUriNotSupported,
    
    [EnumValue("registration_not_supported")]
    RegistrationNotSupported,
    
    [EnumValue("missing_user_code")]
    MissingUserCode,
    
    [EnumValue("invalid_user_code")]
    InvalidUserCode,
    
    [EnumValue("invalid_binding_message")]
    InvalidBindingMessage,
    
    [EnumValue("unknown_user_id")]
    UnknownUserId,
    
    [EnumValue("expired_login_hint_token")]
    ExpiredLoginTokenHint,
    
    [EnumValue("invalid_payload_type")]
    InvalidPayloadType,
    
    [EnumValue("invalid_password")]
    InvalidPassword,
    
    [EnumValue("invalid_device")]
    InvalidDevice,
    
    [EnumValue("invalid_email")]
    InvalidEmail,
    
    [EnumValue("invalid_phone")]
    InvalidPhone,
    
    [EnumValue("invalid_lockout_state")]
    InvalidLockoutState,
    
    [EnumValue("invalid_credentials")]
    InvalidCredentials,
    
    [EnumValue("invalid_session")]
    InvalidSession,
    
    [EnumValue("expired_authentication_session")]
    ExpiredAuthenticationSession,
    
    [EnumValue("invalid_rp")]
    InvalidRp,
    
    [EnumValue("invalid_challenge")]
    InvalidChallenge,
    
    [EnumValue("email_taken")]
    EmailTaken,
    
    [EnumValue("phone_taken")]
    PhoneTaken,
    
    [EnumValue("username_taken")]
    UsernameTaken,
    
    [EnumValue("too_many_failed_login_attempts")]
    TooManyFailedLoginAttempts,
    
    [EnumValue("failed_login_attempt")]
    FailedLoginAttempt,
    
    [EnumValue("untrusted_device")]
    UntrustedDevice,
    
    [EnumValue("unverified_email")]
    UnverifiedEmail,
    
    [EnumValue("account_locked_out")]
    AccountLockedOut,
    
    [EnumValue("blocked_device")]
    BlockedDevice,
    
    [EnumValue("max_emails_count")]
    MaxEmailsCount,
    
    [EnumValue("linked_account_connected")]
    LinkedAccountConnected,
    
    [EnumValue("bad_request")]
    BadRequest,
    
    [EnumValue("method_not_allowed")]
    MethodNotAllowed,
    
    [EnumValue("unsupported_media_type")]
    UnsupportedMediaType,
    
    [EnumValue("unauthotized")]
    Unauthorized,
    
    [EnumValue("forbidden")]
    Forbidden,
    
    [EnumValue("not_found")]
    NotFound,
    
    [EnumValue("too_many_requests")]
    TooManyRequests,
    
    [EnumValue("internal_server_error")]
    InternalServerError,
}