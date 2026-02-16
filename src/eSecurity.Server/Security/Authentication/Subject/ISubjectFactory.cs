namespace eSecurity.Server.Security.Authentication.Subject;

public interface ISubjectFactory<TContext> where TContext : SubjectFactoryContext
{
    public string CreateSubject(TContext context);
}