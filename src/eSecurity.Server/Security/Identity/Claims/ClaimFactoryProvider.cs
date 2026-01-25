namespace eSecurity.Server.Security.Identity.Claims;

public class ClaimFactoryProvider(IServiceProvider serviceProvider) : IClaimFactoryProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ITokenClaimsFactory<TContext, TSource> GetClaimFactory<TContext, TSource>()
        where TContext : TokenClaimsContext where TSource : class
    {
        return _serviceProvider.GetRequiredService<ITokenClaimsFactory<TContext, TSource>>();
    }
}