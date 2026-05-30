namespace eSecurity.Idp.Common.Messaging.Sms;

public interface ISmsBuilderProvider
{
    public ISmsBuilder<TContext> GetBuilder<TContext>()
        where TContext : SmsContext;
}