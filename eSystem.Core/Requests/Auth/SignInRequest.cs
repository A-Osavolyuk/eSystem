using eSystem.Core.Security.Authentication.SignIn;

namespace eSystem.Core.Requests.Auth;

public class SignInRequest
{
    public required SignInType Type { get; set; }
    public required Dictionary<string, object> Credentials { get; set; } = [];
}