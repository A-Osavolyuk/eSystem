namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public sealed class TwoFactorStrategyResolver(IServiceProvider serviceProvider) : ITwoFactorStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITwoFactorStrategy<TContext> Resolve<TContext>(TContext context) where TContext : TwoFactorContext
    {
        var contextType = context.GetType();
        var strategyType = typeof(ITwoFactorStrategy<>).MakeGenericType(contextType);
        return (ITwoFactorStrategy<TContext>)_serviceProvider.GetRequiredService(strategyType);
    }
}