namespace eSecurity.Server.Security.Authorization.Verification;

public interface IVerificationStrategyResolver
{
    public IVerificationStrategy<TContext> Resolve<TContext>(TContext context) where TContext : VerificationContext;
}