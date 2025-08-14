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

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }

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
            
            _ => throw new NotSupportedException("Not supported resend code case")
        };

        await messageService.SendMessageAsync(sender, message, cancellationToken);

        return Result.Success();
    }
}