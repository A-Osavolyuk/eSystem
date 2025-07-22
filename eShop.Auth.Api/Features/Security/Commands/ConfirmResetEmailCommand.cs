using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ConfirmResetEmailCommand(ConfirmResetEmailRequest Request) : IRequest<Result>;

public class ConfirmResetEmailHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IRollbackManager rollbackManager,
    IMessageService messageService) : IRequestHandler<ConfirmResetEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IRollbackManager rollbackManager = rollbackManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(ConfirmResetEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        }
        
        var isEmail = await userManager.IsEmailTakenAsync(request.Request.NewEmail, cancellationToken);

        if (isEmail)
        {
            return Results.BadRequest("This email address is already taken");
        }

        var code = request.Request.Code;
        
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Email, 
            CodeType.Reset, CodeResource.Email, cancellationToken);

        if (!codeResult.Succeeded)
        {
            return codeResult;
        }
        
        var newEmail = request.Request.NewEmail;
        
        var rollback = await rollbackManager.CommitAsync(user, user.Email, RollbackField.Email, cancellationToken);

        if (rollback is null)
        {
            return Results.InternalServerError("Cannot reset email address, rollback was not created.");
        }
        
        var result = await userManager.ResetEmailAsync(user, newEmail, cancellationToken);
        
        var message = new EmailChangedMessage()
        {
            Credentials = new()
            {
                { "Subject", "Email changed" },
                { "To", rollback.Value }
            },
            Payload = new()
            {
                { "UserName", rollback.Value },
                { "Code", rollback.Code },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);
        
        return result;
    }
}