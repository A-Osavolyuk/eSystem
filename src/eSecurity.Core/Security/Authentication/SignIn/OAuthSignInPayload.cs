using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Security.Authentication.SignIn;

public sealed class OAuthSignInPayload : SignInPayload
{
    public required string Email { get; set; }
    public required string ReturnUri { get; set; }
    public required string State { get; set; }
    public required LinkedAccountType Type { get; set; }
}