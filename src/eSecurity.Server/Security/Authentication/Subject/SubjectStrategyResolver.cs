namespace eSecurity.Server.Security.Authentication.Subject;

public sealed class SubjectStrategyResolver(IServiceProvider serviceProvider) : ISubjectStrategyResolver
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ISubjectStrategy<TContext> Resolver<TContext>() where TContext : SubjectStrategyContext
        => _serviceProvider.GetRequiredService<ISubjectStrategy<TContext>>();
}