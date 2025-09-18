using eShop.Auth.Api.Messages.Email;
using eShop.Auth.Api.Messages.Sms;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record SendCodeCommand(SendCodeRequest Request) : IRequest<Result>;

public class SendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<SendCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(SendCodeCommand request, CancellationToken cancellationToken)
    {
        var sender = request.Request.Sender;
        var codeType = request.Request.Type;
        var codeResource = request.Request.Resource;

        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (!user.HasEmail() && sender is SenderType.Email)
        {
            return Results.BadRequest("User does not have an email address to send code via email");
        }

        if (!user.HasPhoneNumber() && sender is SenderType.Sms)
        {
            return Results.BadRequest("User does not have a phone number to send code via SMS");
        }

        if (sender is SenderType.AuthenticatorApp)
        {
            return Result.Success("Code successfully sent. Please, check your authenticator app.");
        }

        var code = await codeManager.GenerateAsync(user, sender, codeType, codeResource, cancellationToken);

        var message = GenerateMessage(request.Request, user, code);
        if (message is null) return Results.BadRequest("Invalid message type");

        await messageService.SendMessageAsync(sender, message, cancellationToken);

        return Result.Success();
    }

    private Message? GenerateMessage(SendCodeRequest request, UserEntity user, string code)
    {
        Message? message = request switch
        {
            { Resource: CodeResource.Email, Type: CodeType.Verify, Sender: SenderType.Email } =>
                new VerifyEmailMessage
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Email verification" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.Email, Type: CodeType.Remove, Sender: SenderType.Email } =>
                new RemoveEmailMessage
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Email remove" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.Account, Type: CodeType.Unlock, Sender: SenderType.Email } =>
                new UnblockAccountMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Account unlock" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.Account, Type: CodeType.Recover, Sender: SenderType.Email } =>
                new RecoverAccountMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Account recovery" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.LinkedAccount, Type: CodeType.Allow, Sender: SenderType.Email } =>
                new AllowLinkedAccountMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", $"Allow {request.Payload["Provider"]} linked account" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "Provider", request.Payload["Provider"] },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.Device, Type: CodeType.Block, Sender: SenderType.Email } =>
                new BlockDeviceMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Device block" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username },
                        { "Ip", request.Payload["IpAddress"] },
                        { "OS", request.Payload["OS"] },
                        { "Device", request.Payload["Device"] },
                        { "Browser", request.Payload["Browser"] }
                    }
                },
            { Resource: CodeResource.Email, Type: CodeType.Current, Sender: SenderType.Email } =>
                new ChangeEmailMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Email change (step one)" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username },
                        { "NewEmail", request.Payload["NewEmail"] }
                    }
                },
            { Resource: CodeResource.LinkedAccount, Type: CodeType.Disallow, Sender: SenderType.Email } =>
                new DisallowLinkedAccountMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", $"Disallow {request.Payload["Provider"]} linked account" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "Provider", request.Payload["Provider"] },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.LinkedAccount, Type: CodeType.Disconnect, Sender: SenderType.Email } =>
                new DisconnectLinkedAccountMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", $"Disconnect {request.Payload["Provider"]} linked account" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "Provider", request.Payload["Provider"] },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.Password, Type: CodeType.Reset, Sender: SenderType.Email } =>
                new ForgotPasswordMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Password reset" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.Passkey, Type: CodeType.Remove, Sender: SenderType.Email } =>
                new RemovePasskeyMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Device block" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username },
                        { "DisplayName", request.Payload["DisplayName"] }
                    }
                },
            { Resource: CodeResource.Email, Type: CodeType.Reset, Sender: SenderType.Email } =>
                new ResetEmailMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Email reset" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.Device, Type: CodeType.Trust, Sender: SenderType.Email } =>
                new TrustDeviceMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Device trust" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username },
                        { "Ip", request.Payload["IpAddress"] },
                        { "OS", request.Payload["OS"] },
                        { "Device", request.Payload["Device"] },
                        { "Browser", request.Payload["Browser"] }
                    }
                },
            { Resource: CodeResource.Device, Type: CodeType.Unblock, Sender: SenderType.Email } =>
                new UnblockDeviceMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Device unblock" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username },
                        { "Ip", request.Payload["IpAddress"] },
                        { "OS", request.Payload["OS"] },
                        { "Device", request.Payload["Device"] },
                        { "Browser", request.Payload["Browser"] }
                    }
                },
            { Resource: CodeResource.Device, Type: CodeType.Verify, Sender: SenderType.Email } =>
                new VerifyDeviceMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Device verification" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username },
                        { "Ip", request.Payload["IpAddress"] },
                        { "OS", request.Payload["OS"] },
                        { "Device", request.Payload["Device"] },
                        { "Browser", request.Payload["Browser"] }
                    }
                },
            { Resource: CodeResource.Email, Type: CodeType.New, Sender: SenderType.Email } =>
                new ConfirmEmailChangeMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Email verification (step two)" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.Username }
                    }
                },
            { Resource: CodeResource.TwoFactor, Type: CodeType.SignIn, Sender: SenderType.Email }
                => new TwoFactorCodeEmailMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Two-factor authentication" }
                    },
                    Payload = new()
                    {
                        { "UserName", user.Username },
                        { "Code", code },
                    },
                },
            { Resource: CodeResource.Provider, Type: CodeType.Subscribe, Sender: SenderType.Email } =>
                new EnableEmailTwoFactorMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                        { "Subject", "Two-factor authentication" }
                    },
                    Payload = new()
                    {
                        { "UserName", user.Username },
                        { "Code", code },
                    },
                },
            { Resource: CodeResource.PhoneNumber, Type: CodeType.Current, Sender: SenderType.Sms } =>
                new ChangePhoneNumberMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    }
                },
            { Resource: CodeResource.PhoneNumber, Type: CodeType.Remove, Sender: SenderType.Sms } =>
                new RemovePhoneNumberMessage()
                {
                    Credentials = new Dictionary<string, string>()
                    {
                        { "To", request.Payload["To"] },
                    },
                    Payload = new()
                    {
                        { "Code", code }
                    }
                },
            { Resource: CodeResource.PhoneNumber, Type: CodeType.Reset, Sender: SenderType.Sms } =>
                new ResetPhoneNumberMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    }
                },
            { Resource: CodeResource.PhoneNumber, Type: CodeType.Verify, Sender: SenderType.Sms } =>
                new VerifyPhoneNumberMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    }
                },
            { Resource: CodeResource.PhoneNumber, Type: CodeType.New, Sender: SenderType.Sms } =>
                new ConfirmPhoneNumberChangeMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    }
                },
            { Resource: CodeResource.TwoFactor, Type: CodeType.SignIn, Sender: SenderType.Sms } =>
                new TwoFactorCodeSmsMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    },
                },
            { Resource: CodeResource.Provider, Type: CodeType.Unsubscribe, Sender: SenderType.Sms } =>
                new EnableSmsTwoFactorMessage()
                {
                    Credentials = new()
                    {
                        { "To", request.Payload["To"] },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    },
                },

            _ => null
        };

        return message;
    }
}