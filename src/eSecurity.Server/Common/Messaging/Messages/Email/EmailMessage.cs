using eSystem.Core.Common.Messaging;

namespace eSecurity.Server.Common.Messaging.Messages.Email;

public class EmailMessage : Message
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
                          {Payload["Content"]}.
                        </p>
                      </div>
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