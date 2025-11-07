using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Access.Verification;
using eSecurity.Security.Identity;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Security.Commands;

public record RemoveEmailCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}

public class RemoveEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<RemoveEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(RemoveEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var userEmail = user.Emails.FirstOrDefault(x => x.Email == request.Email);
        if (userEmail is null) return Results.NotFound($"User doesn't have this email {request.Email}.");

        if (userEmail.Type == EmailType.Primary)
        {
            if (user.CountPasskeys() == 0)
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

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Remove, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;
        
        var result = await userManager.RemoveEmailAsync(user, request.Email, cancellationToken);
        return result;
    }
}