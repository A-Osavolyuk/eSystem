namespace eShop.Auth.Api.Messages.Email;

public class AllowLinkedAccountMessage : Message
{
    public override string Build()
    {
        return $"""
                     <!DOCTYPE html>
                     <html lang="en">
                     <head>
                         <meta charset="UTF-8">
                         <meta name="viewport" content="width=device-width, initial-scale=1.0">
                         <title>Allow {Payload["Provider"]} linked account</title>
                     </head>
                     <body>
                     <div style="border: 1px solid rgb(190, 189, 189); width: 800px; margin: auto; padding: 1px;">
                         <div>
                             <p style="font: bold 24px Arial, sans-serif; color: rgb(141, 66, 212); margin: 30px; text-align: center;">eShop Team</p>
                         </div>
                         <div style="border: 1px solid rgb(190, 189, 189); width: 100%;"></div>
                         <div style="padding: 50px 100px; margin: auto;">
                             <h1 style="font: bold 24px Arial, sans-serif; margin: 0; margin-bottom: 40px;">Allow {Payload["Provider"]} linked account</h1>
                             <p style="font: 16px Arial, sans-serif; margin:0;">Hello, {Payload["UserName"]}!.</p>
                             <br>
                             <p style="font: 16px Arial, sans-serif; margin: 0;">
                                 You attempted to allow {Payload["Provider"]} linked account.
                             </p>
                             <br>
                             <p style="font: 16px Arial, sans-serif; margin: 0;">
                                 To allow linked account, please enter 6-digit code from below.
                             </p>
                             <br>
                             <p style="font: 16px Arial, sans-serif; margin: 0;"> Your allow code: {Payload["Code"]}. Will expire in: 10 mins.</p>
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
    }
}