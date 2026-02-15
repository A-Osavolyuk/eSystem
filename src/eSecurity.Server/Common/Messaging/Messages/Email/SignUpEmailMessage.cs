using eSystem.Core.Common.Messaging;

namespace eSecurity.Server.Common.Messaging.Messages.Email;

public class SignUpEmailMessage : Message
{
    public override string Build()
    {
        return $"""
                <!doctype html>
                <html lang="en">
                  <head>
                    <meta charset="UTF-8" />
                    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
                    <title>{Credentials["Subject"]}</title>
                  </head>
                  <body style="background-color: rgb(230, 229, 229); padding: 20px">
                    <div style="width: 600px; margin: auto; padding: 1px; background-color: white">
                      <div style="padding: 50px 50px; margin: auto">
                        <h1 style="font: bold 24px Arial, sans-serif; margin: 0; margin-bottom: 40px; text-align: center;">
                          {Credentials["Subject"]}
                        </h1>
                        <p style="font: 16px Arial, sans-serif; margin: 0;">
                          Account verification code: {Payload["Code"]}.
                        </p>
                        <p style="font: 16px Arial, sans-serif;margin: 0;">
                          Will expire in: 10 mins.
                        </p>
                        <br/>
                        <p style="font:16px Arial,sans-serif;margin: 0;">
                          Please do not give this code to anyone under any circumstances.
                        </p>
                        <p style=" font: 14px Arial,sans-serif;">
                          In case you have any questions, please
                          <a href="#">contact support here</a>
                        </p>
                      </div>
                    </div>
                  </body>
                </html>
                """;
    }

    public override void Initialize(Dictionary<string, string> payload)
    {
        Credentials = new()
        {
            { "To", payload["To"] },
            { "Subject", "Sign Up" }
        };
        Payload = payload;
    }
}