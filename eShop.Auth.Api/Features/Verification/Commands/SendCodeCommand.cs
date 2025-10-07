using eShop.Domain.Common.Messaging;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Verification.Commands;

public record SendCodeCommand(SendCodeRequest Request) : IRequest<Result>;

public class SendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService,
    MessageRegistry messageRegistry) : IRequestHandler<SendCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
    private readonly MessageRegistry messageRegistry = messageRegistry;

    public async Task<Result> Handle(SendCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (request.Request.Sender is SenderType.AuthenticatorApp)
        {
            return Result.Success("Code successfully sent. Please, check your authenticator app.");
        }

        var sender = request.Request.Sender;
        var action = request.Request.Action;
        var purpose = request.Request.Purpose;
        var payload = request.Request.Payload;

        var code = await codeManager.GenerateAsync(user, sender, action, purpose, cancellationToken);

        payload["Code"] = code;

        if (sender == SenderType.Email) payload["UserName"] = user.Username;

        var metadata = new MessageMetadata()
        {
            Purpose = purpose,
            Sender = sender,
            Action = action,
        };

        var message = messageRegistry.Create(metadata, payload);
        if (message is null) return Results.BadRequest("Invalid message type.");

        await messageService.SendMessageAsync(sender, message, cancellationToken);

        return Result.Success();
    }
}