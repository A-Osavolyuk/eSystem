namespace eShop.EmailSender.Api.Consumers;

public class TwoFactorTokenConsumer(IEmailService emailService)
    : IConsumer<TwoFactorTokenEmailMessage>
{
    private readonly IEmailService emailService = emailService;

    public async Task Consume(ConsumeContext<TwoFactorTokenEmailMessage> context)
    {
        var credentials = context.Message.Credentials;
        var htmlBody = GetEmailBody(credentials.To, context.Message.Token);
        var messageOptions = Mapper.Map(context.Message);
        await emailService.SendMessageAsync(htmlBody, messageOptions);
    }

    private string GetEmailBody(string userName, string code)
    {
        var body = $"""
                    <!DOCTYPE html>
                    <html lang="en">
                    <head>
                        <meta charset="UTF-8">
                        <meta name="viewport" content="width=device-width, initial-scale=1.0">
                        <title>Two-factor authentication</title>
                    </head>
                    <body>
                    <div style="border: 1px solid rgb(190, 189, 189); width: 800px; margin: auto; padding: 1px;">
                        <div style="display: flex; justify-content: center; justify-items: center;">
                            <p style="font: bold 24px Arial, sans-serif; color: rgb(141, 66, 212);">eShop Team</p>
                        </div>
                        <div style="border: 1px solid rgb(190, 189, 189); width: 100%;"></div>
                        <div style="padding: 50px 100px; margin: auto;">
                            <h1 style="font: bold 24px Arial, sans-serif; margin: 0; margin-bottom: 40px;">Two-factor authentication</h1>
                            <p style="font: 16px Arial, sans-serif; margin:0;">Hello, {userName}!.</p>
                            <br>
                            <p style="font: 16px Arial, sans-serif; margin: 0;">
                                 We received a request to sign in with 2-fa.
                            </p>
                            <br>
                            <p style="font: 16px Arial, sans-serif; margin: 0;"> Your 2-fa code: {code}. Will expire in: 10 mins.</p>
                            <br>
                            <p style="font: 16px Arial, sans-serif; margin: 0;">
                                 Please do not give this code to anyone under any circumstances.
                            </p>
                            <br>
                            <p style="font: 16px Arial, sans-serif; margin: 0;">eShop Team.</p>
                            <div style="border: 1px solid rgb(190, 189, 189); width: 100%; margin-top: 40px;"></div>
                            <p style="font: 14px Arial, sans-serif; margin:40px 0;">In case you have any questions, please <a href="#">contact support here</a></p>
                        </div >
                    </div>
                    </body>
                    </html>
                    """;

        return body;
    }
}