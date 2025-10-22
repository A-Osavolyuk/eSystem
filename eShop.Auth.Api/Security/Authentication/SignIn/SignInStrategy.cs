using eShop.Domain.Common.Results;

namespace eShop.Auth.Api.Security.Authentication.SignIn;

public abstract class SignInStrategy
{
    public abstract ValueTask<Result> SignInAsync(Dictionary<string, object> credentials, 
        CancellationToken cancellationToken = default);
}