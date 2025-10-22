using eShop.Domain.Security.Authentication.SignIn;

namespace eShop.Domain.Requests.Auth;

public class SignInRequest
{
    public required SignInType Type { get; set; }
    public required Dictionary<string, object> Credentials { get; set; } = [];
}