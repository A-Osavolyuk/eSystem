using eSystem.Core.Server.Messaging.Sms;
using MassTransit;

namespace eSecurity.Idp.Common.Messaging.Sms;

public sealed class SmsService(IBus bus, ISmsBuilderProvider smsBuilderProvider) : ISmsService
{
    private readonly IBus _bus = bus;
    private readonly ISmsBuilderProvider _smsBuilderProvider = smsBuilderProvider;

    public async ValueTask SendAsync<TContext>(TContext context, CancellationToken cancellationToken = default) 
        where TContext : SmsContext
    {
        var smsBuilder = _smsBuilderProvider.GetBuilder<TContext>();
        var messageBody = smsBuilder.Build(context);
        var request = new SendSmsRequest()
        {
            Body = messageBody,
            Credentials = new SmsCredentials
            {
                To = context.To
            }
        };
        
        var address = new Uri("rabbitmq://localhost/sms-message");
        var endpoint = await _bus.GetSendEndpoint(address);
        await endpoint.Send(request, cancellationToken);
    }
}