namespace eShop.Auth.Api.Security.Authentication;

public abstract class SignInStrategy
{
    public abstract ValueTask<Result> SignInAsync(Dictionary<string, object> credentials, 
        CancellationToken cancellationToken = default);
}