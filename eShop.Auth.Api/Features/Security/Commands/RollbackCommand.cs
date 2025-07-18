using eShop.Domain.Requests;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RollbackCommand(RollbackRequest Request) : IRequest<Result>;

public class RollbackCommandHandler(
    IRollbackManager rollbackManager,
    IUserManager userManager) : IRequestHandler<RollbackCommand, Result>
{
    private readonly IRollbackManager rollbackManager = rollbackManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(RollbackCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var code = request.Request.Code;
        var field = request.Request.Field;
        
        var rollback = await rollbackManager.FindAsync(user, code, field, cancellationToken);

        if (rollback is null)
        {
            return Results.NotFound($"Cannot find rollback.");
        }

        var rollbackResult = field switch
        {
            RollbackField.Email => await userManager.RollbackEmailAsync(user, rollback.Value, cancellationToken),
            RollbackField.RecoveryEmail => await userManager.RollbackRecoveryEmailAsync(user, rollback.Value, cancellationToken),
            RollbackField.PhoneNumber => await userManager.RollbackPhoneNumberAsync(user, rollback.Value, cancellationToken),
            RollbackField.Password => await userManager.RollbackPasswordAsync(user, rollback.Value, cancellationToken),
            _ => throw new NotSupportedException("Rollback is not supported.")
        };

        if (rollbackResult.Succeeded)
        {
            return rollbackResult;
        }
        
        var result = await rollbackManager.RemoveAsync(rollback, cancellationToken);
        
        return result;
    }
}