using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Email.Commands;

public record RemoveEmailCommand(RemoveEmailRequest Request) : IRequest<Result>;

public class RemoveEmailCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IVerificationManager verificationManager) : IRequestHandler<RemoveEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(RemoveEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var email = user.Emails.FirstOrDefault(x => x.Email == request.Request.Email);
        if (email is null) return Results.NotFound($"User doesn't have this email {request.Request.Email}.");
        
        if (email.Type == EmailType.Primary)
        {
            var passkeys = await _passkeyManager.GetAllAsync(user, cancellationToken);
            if (passkeys.Count == 0)
            {
                return Results.BadRequest(
                    "Cannot remove the primary email, because it is the only authentication method");
            }

            if (user.LinkedAccounts.Count >= 1)
            {
                return Results.BadRequest(
                    "Cannot remove the primary email, because there are one or more linked accounts");
            }
        }

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Remove, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var result = await _userManager.RemoveEmailAsync(user, request.Request.Email, cancellationToken);
        return result;
    }
}