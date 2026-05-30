namespace eSecurity.Idp.Common.Messaging.Sms;

public sealed class SmsBuilderProvider(IServiceProvider serviceProvider) : ISmsBuilderProvider
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ISmsBuilder<TContext> GetBuilder<TContext>() where TContext : SmsContext
        => _serviceProvider.GetRequiredService<ISmsBuilder<TContext>>();
}