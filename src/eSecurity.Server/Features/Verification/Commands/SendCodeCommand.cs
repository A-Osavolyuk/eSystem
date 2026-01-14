using eSecurity.Core.Common.Requests;
using eSecurity.Server.Common.Messaging;
using eSecurity.Server.Common.Messaging.Messages.Email;
using eSecurity.Server.Common.Messaging.Messages.Sms;
using eSecurity.Server.Security.Authorization.Access.Codes;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Verification.Commands;

public record SendCodeCommand(SendCodeRequest Request) : IRequest<Result>;

public class SendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<SendCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IMessageService _messageService = messageService;

    public async Task<Result> Handle(SendCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var sender = request.Request.Sender;
        var action = request.Request.Action;
        var purpose = request.Request.Purpose;
        var payload = request.Request.Payload;

        var code = await _codeManager.GenerateAsync(user, sender, action, purpose, cancellationToken);

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
        await _messageService.SendMessageAsync(sender, message, cancellationToken);

        return Results.Ok();
    }
}