using eSecurity.Messaging;
using eSecurity.Messaging.Messages.Email;
using eSecurity.Messaging.Messages.Sms;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.Verification.Commands;

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
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        var sender = request.Request.Sender;
        var action = request.Request.Action;
        var purpose = request.Request.Purpose;
        var payload = request.Request.Payload;

        var code = await codeManager.GenerateAsync(user, sender, action, purpose, cancellationToken);

        payload["Code"] = code;
        payload["UserName"] = user.Username;

        Message? message = sender switch
        {
            SenderType.Email => new VerificationEmailMessage(),
            SenderType.Sms => new VerificationSmsMessage(),
            _ => null
        };
        
        if (message is null) return Results.BadRequest("Invalid message type.");

        message.Initialize(payload);
        await messageService.SendMessageAsync(sender, message, cancellationToken);

        return Result.Success();
    }
}