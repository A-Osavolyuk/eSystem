namespace eSecurity.Core.Security.Authorization.OAuth;

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