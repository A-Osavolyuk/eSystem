namespace eSecurity.Server.Security.Authentication.Subject;

public sealed class SubjectFactoryProvider(
    IServiceProvider serviceProvider) : ISubjectFactoryProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ISubjectFactory<TContext> GetFactory<TContext>() where TContext : SubjectContext
    => _serviceProvider.GetRequiredService<ISubjectFactory<TContext>>();
}