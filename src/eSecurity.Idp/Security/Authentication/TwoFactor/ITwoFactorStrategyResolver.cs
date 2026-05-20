namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public interface ITwoFactorStrategyResolver
{
    public ITwoFactorStrategy<TContext> Resolve<TContext>(TContext context)
        where TContext : TwoFactorContext;
}