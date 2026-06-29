using eSecurity.Idp.Security.Authorization.Scopes.Handlers;

namespace eSecurity.Idp.Security.Authorization.Scopes.Extensions;

public static class ScopeServiceCollectionExtensions
{
    public static void AddScopesProcessing(this IServiceCollection services)
    {
        services.AddScoped<IScopesProcessor, ScopesProcessor>();
        services.AddTransient<IScopeHandler, EmailScopeHandler>();
        services.AddTransient<IScopeHandler, PhoneScopeHandler>();
        services.AddTransient<IScopeHandler, ProfileScopeHandler>();
        services.AddTransient<IScopeHandler, AddressScopeHandler>();
    }
}