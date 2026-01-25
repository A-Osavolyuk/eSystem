namespace eSecurity.Server.Security.Identity.Claims;

public interface IClaimFactoryProvider
{
    public ITokenClaimsFactory<TContext, TSource> GetClaimFactory<TContext, TSource>() 
        where TContext : TokenClaimsContext 
        where TSource : class;
}