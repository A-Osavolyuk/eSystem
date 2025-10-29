using eSystem.Auth.Api.Security.Authorization.Access;
using eSystem.Auth.Api.Security.Identity.Options;
using eSystem.Auth.Api.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;
using eSystem.Core.Security.Identity.Email;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record ResetEmailCommand(ResetEmailRequest Request) : IRequest<Result>;

public class ResetEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IOptions<AccountOptions> options) : IRequestHandler<ResetEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly AccountOptions options = options.Value;

    public async Task<Result> Handle(ResetEmailCommand request, CancellationToken cancellationToken)
    {
        var newEmail = request.Request.NewEmail;

        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
        var userCurrentEmail = user.GetEmail(EmailType.Primary);
        if (userCurrentEmail is null) return Results.BadRequest("User's primary email address is missing");

        if (options.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(request.Request.NewEmail, cancellationToken);
            if (isTaken) return Results.BadRequest("This email address is already taken");
        }

        var resetVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Reset, cancellationToken);

        if (!resetVerificationResult.Succeeded) return resetVerificationResult;
        
        var verifyVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Verify, cancellationToken);

        if (!verifyVerificationResult.Succeeded) return verifyVerificationResult;

        var result = await userManager.ResetEmailAsync(user, userCurrentEmail.Email, newEmail, cancellationToken);
        return result;
    }
}