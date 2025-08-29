using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ChangeRecoveryEmailCommand(ChangeRecoveryEmailRequest Request) : IRequest<Result>;

public class ChangeRecoveryEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IdentityOptions identityOptions) : IRequestHandler<ChangeRecoveryEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(ChangeRecoveryEmailCommand request, CancellationToken cancellationToken)
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

        var currentRecoveryEmailResult = await verificationManager.VerifyAsync(user,
            CodeResource.RecoveryEmail, CodeType.Current, cancellationToken);
        
        if (!currentRecoveryEmailResult.Succeeded) return currentRecoveryEmailResult;
        
        var newRecoveryEmailResult = await verificationManager.VerifyAsync(user,
            CodeResource.RecoveryEmail, CodeType.New, cancellationToken);
        
        if (!newRecoveryEmailResult.Succeeded) return newRecoveryEmailResult;

        var result = await userManager.ChangeRecoveryEmailAsync(user, newRecoveryEmail, cancellationToken);
        return result;
    }
}