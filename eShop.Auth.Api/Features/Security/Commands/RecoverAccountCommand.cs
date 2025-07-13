using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RecoverAccountCommand(RecoverAccountRequest Request) : IRequest<Result>;

public class RecoverAccountCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    ILockoutManager lockoutManager) : IRequestHandler<RecoverAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;

    public async Task<Result> Handle(RecoverAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound("User not found");
        }

        var code = request.Request.Code;
        var verificationResult = await codeManager.VerifyAsync(user, code, SenderType.Email, 
            CodeType.Recover, CodeResource.Account, cancellationToken);

        if (!verificationResult.Succeeded)
        {
            return verificationResult;
        }

        user.FailedLoginAttempts = 0;
        var updateResult = await userManager.UpdateAsync(user, cancellationToken);

        if (!updateResult.Succeeded)
        {
            return updateResult;
        }
        
        var unlockResult = await lockoutManager.UnlockAsync(user, cancellationToken);
        
        return unlockResult;
    }
}