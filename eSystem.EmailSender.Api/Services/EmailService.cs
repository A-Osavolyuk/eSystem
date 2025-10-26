namespace eSystem.EmailSender.Api.Services;

public class EmailService(IOptions<EmailOptions> options) : IEmailService
{
    private readonly EmailOptions options = options.Value;

    public async ValueTask SendMessageAsync(string htmlBody, MessageOptions messageOptions)
    {
        var builder = new BodyBuilder { HtmlBody = htmlBody };
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(options.DisplayName, options.Email));
        emailMessage.To.Add(new MailboxAddress(messageOptions.To, messageOptions.To));
        emailMessage.Subject = messageOptions.Subject;
        emailMessage.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(options.Host, options.Port, false);
        await client.AuthenticateAsync(options.Email, options.Password);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}