using eSystem.Core.Server.Messaging.Email;
using MassTransit;

namespace eSecurity.Idp.Common.Messaging.Email;

public sealed class EmailService(IEmailBuilderProvider builderProvider, IBus bus) : IEmailService
{
    private readonly IBus _bus = bus;
    private readonly IEmailBuilderProvider _builderProvider = builderProvider;

    public async ValueTask SendAsync<TContext>(TContext context, CancellationToken cancellationToken = default)
        where TContext : EmailContext
    {
        var messageBuilder = _builderProvider.GetBuilder<TContext>();
        var messageBody = messageBuilder.Build(context);
        var request = new SendEmailRequest()
        {
            Body = messageBody,
            Credentials = new EmailCredentials
            {
                To = context.To,
                Subject = context.Subject
            }
        };
        
        var address = new Uri("rabbitmq://localhost/email-message");
        var endpoint = await _bus.GetSendEndpoint(address);
        await endpoint.Send(request, cancellationToken);
    }
}