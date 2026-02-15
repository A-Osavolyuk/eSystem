namespace eSecurity.Server.Security.Authentication.Subject;

public interface ISubjectFactory<TContext> where TContext : SubjectContext
{
    public string CreateSubject(TContext context);
}