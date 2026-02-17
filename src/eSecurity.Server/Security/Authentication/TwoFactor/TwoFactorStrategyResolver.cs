namespace eSecurity.Server.Security.Authentication.TwoFactor;

public sealed class TwoFactorStrategyResolver(IServiceProvider serviceProvider) : ITwoFactorStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITwoFactorStrategy<TContext> Resolve<TContext>(TContext context) where TContext : TwoFactorContext
        => _serviceProvider.GetRequiredService<ITwoFactorStrategy<TContext>>();
}