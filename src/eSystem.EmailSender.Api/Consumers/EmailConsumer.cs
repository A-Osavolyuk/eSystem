using eSystem.Core.Server.Messaging.Email;

namespace eSystem.EmailSender.Api.Consumers;

public sealed class EmailConsumer(IEmailService emailService) : IConsumer<SendEmailRequest>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Consume(ConsumeContext<SendEmailRequest> context)
    {
        var message = context.Message;
        var options = new MessageOptions
        {
            Subject = message.Credentials.Subject,
            To = message.Credentials.To,
        };
        
        await _emailService.SendMessageAsync(message.Body, options);
    }
}