using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RemoveRecoveryEmailCommand(RemoveRecoveryEmailRequest Request) : IRequest<Result>;

public class RemoveRecoveryEmailCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<RemoveRecoveryEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(RemoveRecoveryEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var code = await codeManager.GenerateAsync(user, SenderType.Email,
            CodeType.Verify, CodeResource.RecoveryEmail, cancellationToken);

        var message = new RemoveRecoveryEmailMessage()
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
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);
        return Result.Success();
    }
}