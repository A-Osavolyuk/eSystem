using eSystem.Core.Common.Messaging;
using eSystem.EmailSender.Api.Interfaces;
using eSystem.EmailSender.Api.Options;

namespace eSystem.EmailSender.Api.Consumers;

public class EmailConsumer(IEmailService emailService) : IConsumer<MessageRequest>
{
    private readonly IEmailService emailService = emailService;

    public async Task Consume(ConsumeContext<MessageRequest> context)
    {
        var message = context.Message;
        var options = new MessageOptions
        {
            Subject = message.Credentials["Subject"],
            To = message.Credentials["To"],
        };
        
        await emailService.SendMessageAsync(message.Body, options);
    }
}