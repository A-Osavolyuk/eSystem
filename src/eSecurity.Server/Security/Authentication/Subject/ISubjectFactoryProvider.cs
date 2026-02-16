namespace eSecurity.Server.Security.Authentication.Subject;

public interface ISubjectFactoryProvider
{
    public ISubjectFactory<TContext> GetFactory<TContext>() 
        where TContext : SubjectFactoryContext;
}