using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ConfirmRemoveRecoveryEmailCommand(ConfirmRemoveRecoveryEmailRequest Request) : IRequest<Result>;

public class ConfirmRemoveRecoveryEmailCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmRemoveRecoveryEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmRemoveRecoveryEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var code = request.Request.Code;
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Email, 
            CodeType.Remove, CodeResource.RecoveryEmail, cancellationToken);

        if (!codeResult.Succeeded) return codeResult;
        
        var result = await userManager.RemoveRecoveryEmailAsync(user, cancellationToken);
        return result;
    }
}