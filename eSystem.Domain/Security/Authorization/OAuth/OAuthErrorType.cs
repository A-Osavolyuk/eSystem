namespace eSystem.Domain.Security.Authorization.OAuth;

public enum OAuthErrorType
{
    None,
    InternalError,
    InvalidCredentials,
    RemoteError,
    TemporarilyUnavailable,
    UnsupportedProvider,
    Unavailable
}