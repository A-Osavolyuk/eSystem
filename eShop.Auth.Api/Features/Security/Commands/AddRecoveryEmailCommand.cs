using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record AddRecoveryEmailCommand(AddRecoveryEmailRequest Request) : IRequest<Result>;

public class AddRecoveryEmailCommandHandler(
    IUserManager userManager,
    IMessageService messageService,
    ICodeManager codeManager) : IRequestHandler<AddRecoveryEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IMessageService messageService = messageService;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(AddRecoveryEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }
        
        var isTaken = await userManager.IsEmailTakenAsync(request.Request.RecoveryEmail, cancellationToken);

        if (isTaken)
        {
            return Results.BadRequest("This email address is already taken");
        }
        
        var result = await userManager.AddRecoveryEmailAsync(user, request.Request.RecoveryEmail, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        var code = await codeManager.GenerateAsync(user, SenderType.Email,
            CodeType.Verify, CodeResource.RecoveryEmail, cancellationToken);

        var message = new AddRecoveryEmailMessage()
        {
            Credentials = new()
            {
                { "Subject", "Recovery email verification" },
                { "To", request.Request.RecoveryEmail },
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