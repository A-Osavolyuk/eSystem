using eSystem.Auth.Api.Security.Identity.Options;
using eSystem.Auth.Api.Security.Identity.Privacy;
using eSystem.Auth.Api.Security.Identity.User;

namespace eSystem.Auth.Api.Security.Identity;

public static class IdentityExtensions
{
    public static void AddIdentity(this IHostApplicationBuilder builder, Action<IdentityBuilder> configurator)
    {
        var identityBuilder = new IdentityBuilder(builder.Services);
        configurator(identityBuilder);
        
        builder.Services.AddScoped<IUserManager, UserManager>();
        builder.Services.AddScoped<IPersonalDataManager, PersonalDataManager>();
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