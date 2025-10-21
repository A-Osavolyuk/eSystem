namespace eShop.Auth.Api.Extensions;

public static class IdentityExtensions
{
    public static void AddIdentity(this IServiceCollection services, Action<IdentityBuilder> configurator)
    {
        var builder = new IdentityBuilder(services);
        configurator(builder);
    }
}


public class IdentityBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
    public void ConfigurePassword(Action<PasswordOptions> configure) => Services.Configure(configure);
    public void ConfigureAccount(Action<AccountOptions> configure) => Services.Configure(configure);
    public void ConfigureSignIn(Action<SignInOptions> configure) => Services.Configure(configure);
    public void ConfigureCode(Action<CodeOptions> configure) => Services.Configure(configure);
}