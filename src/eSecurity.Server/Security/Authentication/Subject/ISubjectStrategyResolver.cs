using eSecurity.Server.Security.Authentication.OpenIdConnect.Client;

namespace eSecurity.Server.Security.Authentication.Subject;

public interface ISubjectStrategyResolver
{
    public ISubjectStrategy<TContext> Resolver<TContext>() 
        where TContext : SubjectStrategyContext;
}