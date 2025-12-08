using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Account.Commands;

public record RecoverAccountCommand(RecoverAccountRequest Request) : IRequest<Result>;

public class RecoverAccountCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IVerificationManager verificationManager) : IRequestHandler<RecoverAccountCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(RecoverAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userPrimaryEmail = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (userPrimaryEmail is null) return Results.BadRequest("User does not have a primary email.");
        if (!userPrimaryEmail.IsVerified) return Results.BadRequest("User's primary email is not verified.");

        var userRecoveryEmail = await _emailManager.FindByTypeAsync(user, EmailType.Recovery, cancellationToken);
        if (userRecoveryEmail is null) return Results.BadRequest("User does not have a recovery email.");
        if (!userRecoveryEmail.IsVerified) return Results.BadRequest("User's recovery email is not verified.");

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Account, ActionType.Recover, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        userPrimaryEmail.Type = EmailType.Secondary;
        userPrimaryEmail.UpdateDate = DateTimeOffset.UtcNow;

        userRecoveryEmail.Type = EmailType.Primary;
        userRecoveryEmail.UpdateDate = DateTimeOffset.UtcNow;

        var updateResult = await _userManager.UpdateAsync(user, cancellationToken);
        return updateResult;
    }
}