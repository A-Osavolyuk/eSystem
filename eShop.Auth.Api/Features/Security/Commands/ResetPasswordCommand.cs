using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<Result>;

public sealed class ResetPasswordCommandHandler(
    IUserManager userManager,
    IRollbackManager rollbackManager,
    IMessageService messageService) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRollbackManager rollbackManager = rollbackManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var rollback = await rollbackManager.SaveAsync(user, user.PasswordHash, RollbackField.Password, cancellationToken);

        if (rollback is null)
        {
            return Results.InternalServerError("Cannot reset password, rollback was not created.");
        }
        
        var resetResult = await userManager.ResetPasswordAsync(user, request.Request.NewPassword, cancellationToken);

        var message = new PasswordChangedMessage()
        {
            Credentials = new()
            {
                { "Subject", "Password changed" },
                { "To", user.Email }
            },
            Payload = new()
            {
                { "UserName", user.UserName },
                { "Code", rollback.Code },
            }
        };
        
        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);
        
        return resetResult;
    }
}