using eSystem.Core.Security.Authentication.SignIn;

namespace eSystem.Core.Requests.Auth;

public class SignInRequest
{
    public required SignInPayload Payload { get; set; }
}