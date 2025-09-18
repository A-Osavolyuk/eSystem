using eShop.Domain.Common.Messaging;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ResendCodeCommand(ResendCodeRequest Request) : IRequest<Result>;

public class ResendCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService,
    MessageRegistry messageRegistry) : IRequestHandler<ResendCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;
    private readonly MessageRegistry messageRegistry = messageRegistry;

    public async Task<Result> Handle(ResendCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (request.Request.Sender is SenderType.AuthenticatorApp)
        {
            return Result.Success("Code successfully sent. Please, check your authenticator app.");
        }

        var sender = request.Request.Sender;
        var type = request.Request.Type;
        var resource = request.Request.Resource;
        var payload = request.Request.Payload;

        var code = await codeManager.GenerateAsync(user, sender, type, resource, cancellationToken);
        payload["Code"] = code;

        if (sender == SenderType.Email) payload["UserName"] = user.Username;

        var metadata = new MessageMetadata()
        {
            Sender = sender,
            Type = type,
            Resource = resource
        };

        var message = messageRegistry.Create(metadata, payload);
        if (message is null) return Results.BadRequest("Invalid message type.");
        
        await messageService.SendMessageAsync(sender, message, cancellationToken);
        return Result.Success();
    }
}