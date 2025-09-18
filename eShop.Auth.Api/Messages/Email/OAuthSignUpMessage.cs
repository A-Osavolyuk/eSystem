namespace eShop.Auth.Api.Messages.Email;

public class OAuthSignUpMessage : Message
{
    public override string Build()
    {
        return $"""
                         <!DOCTYPE html>
                         <html lang="en">
                         <head>
                             <meta charset="UTF-8">
                             <meta name="viewport" content="width=device-width, initial-scale=1.0">
                             <title>{Payload["ProviderName"]} sign in</title>
                         </head>
                         <body>
                         <div style="border: 1px solid rgb(190, 189, 189); width: 800px; margin: auto; padding: 1px;">
                             <div>
                                 <p style="font: bold 24px Arial, sans-serif; color: rgb(141, 66, 212); margin: 30px; text-align: center;">eShop Team</p>
                             </div>
                             <div style="border: 1px solid rgb(190, 189, 189); width: 100%;"></div>
                             <div style="padding: 50px 100px; margin: auto;">
                                 <h1 style="font: bold 24px Arial, sans-serif; margin: 0; margin-bottom: 40px;">
                                    Sign in with {Payload["ProviderName"]}
                                 </h1>
                                 <br>
                                 <p style="font: 16px Arial, sans-serif; margin:0;">Hello, {Payload["UserName"]}!.</p>
                                 <br>
                                 <p style="font: 16px Arial, sans-serif; margin: 1px;">
                                    Your account successfully signed up with {Payload["ProviderName"]}.
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
    }

    public override void Initialize(Dictionary<string, string> payload)
    {
        Payload = payload;
    }
}