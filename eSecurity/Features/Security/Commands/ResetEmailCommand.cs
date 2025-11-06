using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.Options;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;
using eSystem.Core.Security.Identity.Email;

namespace eSecurity.Features.Security.Commands;

public record ResetEmailCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string NewEmail { get; set; }
}

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
        var newEmail = request.NewEmail;

        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");
        
        var userCurrentEmail = user.GetEmail(EmailType.Primary);
        if (userCurrentEmail is null) return Results.BadRequest("User's primary email address is missing");

        if (options.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(request.NewEmail, cancellationToken);
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