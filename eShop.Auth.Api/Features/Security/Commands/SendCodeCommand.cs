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
        var codeType = request.Request.CodeType;
        var codeResource = request.Request.CodeResource;
        var payload = request.Request.Payload;

        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        var code = await codeManager.GenerateAsync(user, sender, codeType, codeResource, cancellationToken);

        Message message = request.Request switch
        {
            { CodeResource: CodeResource.Email, CodeType: CodeType.Verify, Sender: SenderType.Email } =>
                new AccountRegisteredMessage
                {
                    Credentials = new()
                    {
                        { "To", user.Email },
                        { "Subject", "Account registered" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName }
                    }
                },
            { CodeResource: CodeResource.Account, CodeType: CodeType.Unlock, Sender: SenderType.Email } =>
                new AccountUnlockMessage()
                {
                    Credentials = new()
                    {
                        { "To", user.Email },
                        { "Subject", "Account unlock" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName }
                    }
                },
            { CodeResource: CodeResource.RecoveryEmail, CodeType: CodeType.Verify, Sender: SenderType.Email } =>
                new AddRecoveryEmailMessage()
                {
                    Credentials = new()
                    {
                        { "Subject", "Recovery email verification" },
                        { "To", payload["RecoveryEmail"] },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName },
                    }
                },
            { CodeResource: CodeResource.LinkedAccount, CodeType: CodeType.Allow, Sender: SenderType.Email } =>
                new AllowLinkedAccountMessage()
                {
                    Credentials = new()
                    {
                        { "To", user!.Email },
                        { "Subject", $"Allow {payload["Provider"]} linked account" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "Provider", payload["Provider"] },
                        { "UserName", user.UserName }
                    }
                },
            { CodeResource: CodeResource.Device, CodeType: CodeType.Block, Sender: SenderType.Email } =>
                new BlockDeviceMessage()
                {
                    Credentials = new()
                    {
                        { "To", user!.Email },
                        { "Subject", "Device block" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName },
                        { "Ip", payload["IpAddress"] },
                        { "OS", payload["OS"] },
                        { "Device", payload["Device"] },
                        { "Browser", payload["Browser"] }
                    }
                },
            { CodeResource: CodeResource.Email, CodeType: CodeType.Current, Sender: SenderType.Email } =>
                new ChangeEmailMessage()
                {
                    Credentials = new()
                    {
                        { "To", user.Email },
                        { "Subject", "Email change (step one)" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName },
                        { "NewEmail", payload["NewEmail"] }
                    }
                },
            { CodeResource: CodeResource.LinkedAccount, CodeType: CodeType.Disallow, Sender: SenderType.Email } =>
                new DisallowLinkedAccountMessage()
                {
                    Credentials = new()
                    {
                        { "To", user!.Email },
                        { "Subject", $"Disallow {payload["Provider"]} linked account" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "Provider", payload["Provider"] },
                        { "UserName", user.UserName }
                    }
                },
            { CodeResource: CodeResource.LinkedAccount, CodeType: CodeType.Disconnect, Sender: SenderType.Email } =>
                new DisconnectLinkedAccountMessage()
                {
                    Credentials = new()
                    {
                        { "To", user!.Email },
                        { "Subject", $"Disconnect {payload["Provider"]} linked account" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "Provider", payload["Provider"] },
                        { "UserName", user.UserName }
                    }
                },
            { CodeResource: CodeResource.Password, CodeType: CodeType.Reset, Sender: SenderType.Email } =>
                new ForgotPasswordMessage()
                {
                    Credentials = new()
                    {
                        { "To", user.Email },
                        { "Subject", "Password reset" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName }
                    }
                },
            { CodeResource: CodeResource.Passkey, CodeType: CodeType.Remove, Sender: SenderType.Email } =>
                new RemovePasskeyMessage()
                {
                    Credentials = new()
                    {
                        { "To", user!.Email },
                        { "Subject", "Device block" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName },
                        { "DisplayName", payload["DisplayName"] }
                    }
                },
            { CodeResource: CodeResource.RecoveryEmail, CodeType: CodeType.Remove, Sender: SenderType.Email } =>
                new RemoveRecoveryEmailMessage()
                {
                    Credentials = new()
                    {
                        { "Subject", "Recovery email remove" },
                        { "To", user.RecoveryEmail! },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName },
                    }
                },
            { CodeResource: CodeResource.Email, CodeType: CodeType.Reset, Sender: SenderType.Email } =>
                new ResetEmailMessage()
                {
                    Credentials = new()
                    {
                        { "To", payload["NewEmail"] },
                        { "Subject", "Email reset" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName }
                    }
                },
            { CodeResource: CodeResource.Device, CodeType: CodeType.Trust, Sender: SenderType.Email } =>
                new TrustDeviceMessage()
                {
                    Credentials = new()
                    {
                        { "To", user!.Email },
                        { "Subject", "Device trust" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName },
                        { "Ip", payload["IpAddress"] },
                        { "OS", payload["OS"] },
                        { "Device", payload["Device"] },
                        { "Browser", payload["Browser"] }
                    }
                },
            { CodeResource: CodeResource.Device, CodeType: CodeType.Unblock, Sender: SenderType.Email } =>
                new UnblockDeviceMessage()
                {
                    Credentials = new()
                    {
                        { "To", user!.Email },
                        { "Subject", "Device unblock" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName },
                        { "Ip", payload["IpAddress"] },
                        { "OS", payload["OS"] },
                        { "Device", payload["Device"] },
                        { "Browser", payload["Browser"] }
                    }
                },
            { CodeResource: CodeResource.Device, CodeType: CodeType.Verify, Sender: SenderType.Email } =>
                new VerifyDeviceMessage()
                {
                    Credentials = new()
                    {
                        { "To", user!.Email },
                        { "Subject", "Device verification" }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName },
                        { "Ip", payload["IpAddress"] },
                        { "OS", payload["OS"] },
                        { "Device", payload["Device"] },
                        { "Browser", payload["Browser"] }
                    }
                },
            { CodeResource: CodeResource.Email, CodeType: CodeType.New, Sender: SenderType.Email } =>
                new VerifyEmailMessage()
                {
                    Credentials = new()
                    {
                        { "To", payload["NewEmail"] },
                        { "Subject", "Email verification (step two)" },
                    },
                    Payload = new()
                    {
                        { "Code", code },
                        { "UserName", user.UserName }
                    }
                },
            { CodeResource: CodeResource.PhoneNumber, CodeType: CodeType.Current, Sender: SenderType.Sms } =>
                new ChangePhoneNumberMessage()
                {
                    Credentials = new()
                    {
                        { "PhoneNumber", user.PhoneNumber! }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    }
                },
            { CodeResource: CodeResource.PhoneNumber, CodeType: CodeType.Remove, Sender: SenderType.Sms } =>
                new RemovePhoneNumberMessage()
                {
                    Credentials = new Dictionary<string, string>()
                    {
                        { "PhoneNumber", user.PhoneNumber! },
                    },
                    Payload = new()
                    {
                        { "Code", code }
                    }
                },
            { CodeResource: CodeResource.PhoneNumber, CodeType: CodeType.Reset, Sender: SenderType.Sms } =>
                new ResetPhoneNumberMessage()
                {
                    Credentials = new()
                    {
                        { "PhoneNumber", payload["NewPhoneNumber"] }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    }
                },
            { CodeResource: CodeResource.PhoneNumber, CodeType: CodeType.Verify, Sender: SenderType.Sms } =>
                new VerifyPhoneNumberMessage()
                {
                    Credentials = new()
                    {
                        { "PhoneNumber", user.PhoneNumber! }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    }
                },
            { CodeResource: CodeResource.PhoneNumber, CodeType: CodeType.New, Sender: SenderType.Sms } =>
                new VerifyPhoneNumberMessage()
                {
                    Credentials = new()
                    {
                        { "PhoneNumber", payload["NewPhoneNumber"] }
                    },
                    Payload = new()
                    {
                        { "Code", code },
                    }
                },

            _ => throw new NotSupportedException("Not supported resend code case")
        };

        await messageService.SendMessageAsync(sender, message, cancellationToken);

        return Result.Success();
    }
}