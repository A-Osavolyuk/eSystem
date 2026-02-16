using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Mediator;

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
        if (user is null) return Results.NotFound("User not found.");

        var userPrimaryEmail = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (userPrimaryEmail is null || !userPrimaryEmail.IsVerified)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidEmail,
                Description = "Invalid primary email"
            });
        }

        var userRecoveryEmail = await _emailManager.FindByTypeAsync(user, EmailType.Recovery, cancellationToken);
        if (userRecoveryEmail is null || !userRecoveryEmail.IsVerified)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidEmail,
                Description = "Invalid recovery email"
            });
        }

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Account, ActionType.Recover, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        userPrimaryEmail.Type = EmailType.Secondary;
        userRecoveryEmail.Type = EmailType.Primary;

        var updateResult = await _userManager.UpdateAsync(user, cancellationToken);
        return updateResult;
    }
}