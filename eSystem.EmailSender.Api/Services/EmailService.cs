namespace eSystem.EmailSender.Api.Services;

public class EmailService(IOptions<EmailOptions> options) : IEmailService
{
    private readonly EmailOptions _options = options.Value;

    public async ValueTask SendMessageAsync(string htmlBody, MessageOptions messageOptions)
    {
        var builder = new BodyBuilder { HtmlBody = htmlBody };
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(_options.DisplayName, _options.Email));
        emailMessage.To.Add(new MailboxAddress(messageOptions.To, messageOptions.To));
        emailMessage.Subject = messageOptions.Subject;
        emailMessage.Body = builder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, false);
        await client.AuthenticateAsync(_options.Email, _options.Password);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}