namespace eSecurity.Server.Security.Authorization.Verification;

public sealed class VerificationStrategyResolver(IServiceProvider serviceProvider) : IVerificationStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IVerificationStrategy<TContext> Resolve<TContext>(TContext context) where TContext : VerificationContext
        => _serviceProvider.GetRequiredService<IVerificationStrategy<TContext>>();
}