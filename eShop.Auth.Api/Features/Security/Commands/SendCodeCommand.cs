using eShop.Auth.Api.Messages;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

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

        if (!user.HasEmail() && request.Request.Sender is SenderType.Email)
        {
            return Results.BadRequest("User does not have an email address to send code via email");
        }

        if (!user.HasPhoneNumber() && request.Request.Sender is SenderType.Sms)
        {
            return Results.BadRequest("User does not have a phone number to send code via SMS");
        }

        if (request.Request.Sender is SenderType.AuthenticatorApp)
        {
            return Result.Success("Code successfully sent. Please, check your authenticator app.");
        }

        var sender = request.Request.Sender;
        var codeType = request.Request.Type;
        var codeResource = request.Request.Resource;
        var payload = request.Request.Payload;

        var code = await codeManager.GenerateAsync(user, sender, codeType, codeResource, cancellationToken);

        payload["Code"] = code;

        if (sender == SenderType.Email) payload["UserName"] = user.Username;

        var metadata = new MessageMetadata()
        {
            Resource = codeResource,
            Sender = sender,
            Type = codeType,
        };

        var message = messageRegistry.Create(metadata, payload);
        if (message is null) return Results.BadRequest("Invalid message type");

        await messageService.SendMessageAsync(sender, message, cancellationToken);

        return Result.Success();
    }
}