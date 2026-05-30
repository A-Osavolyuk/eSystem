namespace eSecurity.Idp.Common.Messaging.Email;

public sealed class EmailBuilderProvider(IServiceProvider serviceProvider) : IEmailBuilderProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public IEmailBuilder<TContext> GetBuilder<TContext>() where TContext : EmailContext
        => _serviceProvider.GetRequiredService<IEmailBuilder<TContext>>();
}