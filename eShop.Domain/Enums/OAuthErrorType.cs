namespace eShop.Domain.Enums;

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