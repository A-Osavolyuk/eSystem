using eSecurity.Core.Security.Authorization.OAuth;

namespace eSecurity.Core.Security.Authentication.SignIn;

public sealed class OAuthSignInPayload : SignInPayload
{
    public required Guid SessionId { get; set; }
    public required string Email { get; set; }
    public required string ReturnUri { get; set; }
    public required string Token { get; set; }
    public required LinkedAccountType LinkedAccount { get; set; }
}