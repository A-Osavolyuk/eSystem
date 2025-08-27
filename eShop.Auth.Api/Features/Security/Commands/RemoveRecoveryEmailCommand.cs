using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RemoveRecoveryEmailCommand(RemoveRecoveryEmailRequest Request) : IRequest<Result>;

public class RemoveRecoveryEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<RemoveRecoveryEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(RemoveRecoveryEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.RecoveryEmail, CodeType.Remove, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await userManager.RemoveRecoveryEmailAsync(user, cancellationToken);
        return result;
    }
}