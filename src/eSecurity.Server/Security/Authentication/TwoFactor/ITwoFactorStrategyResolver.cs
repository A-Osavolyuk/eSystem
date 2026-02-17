namespace eSecurity.Server.Security.Authentication.TwoFactor;

public interface ITwoFactorStrategyResolver
{
    public ITwoFactorStrategy<TContext> Resolve<TContext>(TContext context)
        where TContext : TwoFactorContext;
}