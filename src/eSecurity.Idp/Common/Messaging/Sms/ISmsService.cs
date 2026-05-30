namespace eSecurity.Idp.Common.Messaging.Sms;

public interface ISmsService
{
    public ValueTask SendAsync<TContext>(TContext context, CancellationToken cancellationToken = default)
        where TContext : SmsContext;
}