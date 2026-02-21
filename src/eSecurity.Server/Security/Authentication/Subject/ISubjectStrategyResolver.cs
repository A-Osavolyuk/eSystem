namespace eSecurity.Server.Security.Authentication.Subject;

public interface ISubjectStrategyResolver
{
    public ISubjectStrategy<TContext> Resolver<TContext>() 
        where TContext : SubjectStrategyContext;
}