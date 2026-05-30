namespace eSecurity.Idp.Common.Messaging.Sms;

public interface ISmsBuilder<in TContext> where TContext : SmsContext
{
    public string Build(TContext context);
}