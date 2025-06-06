namespace eShop.EmailSender.Api.Consumers;

public class AccountRecoveryConsumer(IEmailService emailService) : IConsumer<AccountRecoveryEmailMessage>
{
    private readonly IEmailService emailService = emailService;

    public async Task Consume(ConsumeContext<AccountRecoveryEmailMessage> context)
    {
        var credentials = context.Message.Credentials;
        var code = context.Message.Code;
        var htmlBody = GetEmailBody(credentials.To, code);
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
                             <title>Account recovery</title>
                         </head>
                         <body>
                         <div style="border: 1px solid rgb(190, 189, 189); width: 800px; margin: auto; padding: 1px;">
                            <div style="padding-left: 30px">
                                <p style="font: bold 24px Arial, sans-serif; color: rgb(141, 66, 212); margin: 30px; text-align: center;">eShop Team</p>
                            </div>
                             <div style="border: 1px solid rgb(190, 189, 189); width: 100%;"></div>
                             <div style="padding: 50px 100px; margin: auto;">
                                 <h1 style="font: bold 24px Arial, sans-serif; margin: 0; margin-bottom: 40px;">Account recovery</h1>
                                 <p style="font: 16px Arial, sans-serif; margin: 0;">Hello, {userName}!.</p>
                                 <br/>
                                 <p style="font: 16px Arial, sans-serif; margin: 0;">
                                    Your account was blocked due to too many login attempts. 
                                    Too recover your account, please enter code from below.
                                 </p>
                                 <br/>
                                 <p style="font: 16px Arial, sans-serif; margin: 0;"> 
                                    Your recovery code: {code}. Will expire in: 10 mins.
                                 </p>
                                 <br/>
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