using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public interface ITwoFactorStrategy<in TContext> 
    where TContext : TwoFactorContext
{
    public ValueTask<Result> ExecuteAsync(TContext context, CancellationToken cancellationToken = default);
}