using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record VerifyRecoveryEmailCommand(VerifyRecoveryEmailRequest Request) : IRequest<Result>;

public class VerifyRecoveryEmailCommandHandler(
    ICodeManager codeManager, 
    IUserManager userManager) : IRequestHandler<VerifyRecoveryEmailCommand, Result>
{
    private readonly ICodeManager codeManager = codeManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(VerifyRecoveryEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
        var codeVerifiedResult = await codeManager.VerifyAsync(user, request.Request.Code, 
            SenderType.Email, CodeType.Verify, CodeResource.RecoveryEmail, cancellationToken);

        if (!codeVerifiedResult.Succeeded) return codeVerifiedResult;

        var result = await userManager.VerifyRecoveryEmailAsync(user, cancellationToken);
        return result;
    }
}