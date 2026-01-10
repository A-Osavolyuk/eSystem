using eSystem.Core.Common.Messaging;

namespace eSystem.EmailSender.Api.Consumers;

public class EmailConsumer(IEmailService emailService) : IConsumer<MessageRequest>
{
    private readonly IEmailService _emailService = emailService;

    public async Task Consume(ConsumeContext<MessageRequest> context)
    {
        var message = context.Message;
        var options = new MessageOptions
        {
            Subject = message.Credentials["Subject"],
            To = message.Credentials["To"],
        };
        
        await _emailService.SendMessageAsync(message.Body, options);
    }
}