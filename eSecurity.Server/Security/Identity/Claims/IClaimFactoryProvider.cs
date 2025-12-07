namespace eSecurity.Server.Security.Identity.Claims;

public interface IClaimFactoryProvider
{
    public IClaimFactory<TContext> GetClaimFactory<TContext>() where TContext : ClaimsContext;
}