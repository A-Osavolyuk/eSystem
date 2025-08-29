using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ResetRecoveryEmailCommand(ResetRecoveryEmailRequest Request) : IRequest<Result>;

public class ResetRecoveryEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IdentityOptions identityOptions) : IRequestHandler<ResetRecoveryEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(ResetRecoveryEmailCommand request, CancellationToken cancellationToken)
    {
        var newRecoveryEmail = request.Request.NewRecoveryEmail;
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        if (!user.HasRecoveryEmail()) return Results.BadRequest("User does not have a recovery email.");

        if (identityOptions.Account.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(newRecoveryEmail, cancellationToken);
            if (!isTaken) return Results.BadRequest("This email address is already taken.");
        }

        var verificationResult = await verificationManager.VerifyAsync(user,
            CodeResource.RecoveryEmail, CodeType.Reset, cancellationToken);
        
        if (!verificationResult.Succeeded) return verificationResult;

        var result = await userManager.ResetRecoveryEmailAsync(user, newRecoveryEmail, cancellationToken);
        return result;
    }
}