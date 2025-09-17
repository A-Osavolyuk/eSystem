using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record ResetEmailCommand(ResetEmailRequest Request) : IRequest<Result>;

public class ResetEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IdentityOptions identityOptions) : IRequestHandler<ResetEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(ResetEmailCommand request, CancellationToken cancellationToken)
    {
        var newEmail = request.Request.NewEmail;

        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
        var userCurrentEmail = user.PrimaryEmail;
        if (userCurrentEmail is null) return Results.BadRequest("User's primary email address is missing");

        if (identityOptions.Account.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(request.Request.NewEmail, cancellationToken);
            if (isTaken) return Results.BadRequest("This email address is already taken");
        }

        var verificationResult = await verificationManager.VerifyAsync(user,
            CodeResource.Email, CodeType.Reset, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await userManager.ResetEmailAsync(user, userCurrentEmail.Email, newEmail, cancellationToken);
        return result;
    }
}