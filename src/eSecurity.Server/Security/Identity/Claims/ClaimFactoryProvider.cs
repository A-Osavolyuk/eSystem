namespace eSecurity.Server.Security.Identity.Claims;

public class ClaimFactoryProvider(IServiceProvider serviceProvider) : IClaimFactoryProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IClaimFactory<TContext> GetClaimFactory<TContext>() where TContext : ClaimsContext
        => _serviceProvider.GetRequiredService<IClaimFactory<TContext>>();
}