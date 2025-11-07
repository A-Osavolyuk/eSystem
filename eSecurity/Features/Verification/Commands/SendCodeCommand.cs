using eSecurity.Common.Messaging;
using eSecurity.Common.Messaging.Messages.Email;
using eSecurity.Common.Messaging.Messages.Sms;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Access.Codes;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Features.Verification.Commands;

public record SendCodeCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required SenderType Sender { get; set; }
    public required ActionType Action { get; set; }
    public required PurposeType Purpose { get; set; }
    public required Dictionary<string, string> Payload { get; set; } = [];
}

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
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");

        var sender = request.Sender;
        var action = request.Action;
        var purpose = request.Purpose;
        var payload = request.Payload;

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