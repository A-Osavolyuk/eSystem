using eShop.Domain.Common.Messaging;

namespace eShop.EmailSender.Api.Consumers;

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
            UserName = message.Credentials["UserName"],
        };
        
        await emailService.SendMessageAsync(message.Body, options);
    }
}