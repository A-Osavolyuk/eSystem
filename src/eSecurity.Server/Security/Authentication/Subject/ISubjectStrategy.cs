namespace eSecurity.Server.Security.Authentication.Subject;

public interface ISubjectStrategy<TContext> where TContext : SubjectStrategyContext
{
    public ValueTask<TypedResult<string>> ExecuteAsync(TContext context, 
        CancellationToken cancellationToken = default);
}