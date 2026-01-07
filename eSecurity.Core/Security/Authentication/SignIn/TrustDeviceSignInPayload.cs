namespace eSecurity.Core.Security.Authentication.SignIn;

public sealed class TrustDeviceSignInPayload : SignInPayload
{
    public required Guid Sid { get; set; }
}