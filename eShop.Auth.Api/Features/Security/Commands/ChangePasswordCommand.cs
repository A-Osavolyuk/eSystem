using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest<Result>;

public sealed class ChangePasswordCommandHandler(
    IUserManager userManager,
    IRollbackManager rollbackManager,
    IMessageService messageService) : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRollbackManager rollbackManager = rollbackManager;
    private readonly IMessageService messageService = messageService;

    async Task<Result>
        IRequestHandler<ChangePasswordCommand, Result>.Handle(ChangePasswordCommand request,
            CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }

        var isCorrectPassword = await userManager.CheckPasswordAsync(user, request.Request.CurrentPassword, cancellationToken);
        
        if (!isCorrectPassword)
        {
            return Results.BadRequest($"Wrong password.");
        }

        var rollback = await rollbackManager.CommitAsync(user, user.PasswordHash, RollbackField.Password, cancellationToken);

        if (rollback is null)
        {
            return Results.InternalServerError("Cannot change password, rollback was not created.");
        }
        
        var result = await userManager.ChangePasswordAsync(user, request.Request.NewPassword, cancellationToken);
        
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

        return result;
    }
}