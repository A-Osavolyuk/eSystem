using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Requests.Auth;
using eSystem.Domain.Security.Verification;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record RecoverAccountCommand(RecoverAccountRequest Request) : IRequest<Result>;

public class RecoverAccountCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<RecoverAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(RecoverAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userPrimaryEmail = user.GetEmail(EmailType.Primary);
        if (userPrimaryEmail is null) return Results.BadRequest("User does not have a primary email.");
        if (!userPrimaryEmail.IsVerified) return Results.BadRequest("User's primary email is not verified.");

        var userRecoveryEmail = user.GetEmail(EmailType.Recovery);
        if (userRecoveryEmail is null) return Results.BadRequest("User does not have a recovery email.");
        if (!userRecoveryEmail.IsVerified) return Results.BadRequest("User's recovery email is not verified.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Account, ActionType.Recover, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        userPrimaryEmail.Type = EmailType.Secondary;
        userPrimaryEmail.UpdateDate = DateTimeOffset.UtcNow;

        userRecoveryEmail.Type = EmailType.Primary;
        userRecoveryEmail.UpdateDate = DateTimeOffset.UtcNow;

        var updateResult = await userManager.UpdateAsync(user, cancellationToken);
        return updateResult;
    }
}