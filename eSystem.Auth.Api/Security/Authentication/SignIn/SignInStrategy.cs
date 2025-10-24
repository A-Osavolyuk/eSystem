namespace eSystem.Auth.Api.Security.Authentication.SignIn;

public abstract class SignInStrategy
{
    public abstract ValueTask<Result> SignInAsync(Dictionary<string, object> credentials, 
        CancellationToken cancellationToken = default);
}