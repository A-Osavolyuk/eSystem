using eShop.Auth.Api.Security.Identity.Options;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ChangeEmailCommand(ChangeEmailRequest Request) : IRequest<Result>;

public sealed class RequestChangeEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IOptions<AccountOptions> options) : IRequestHandler<ChangeEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly AccountOptions options = options.Value;

    public async Task<Result> Handle(ChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");

        if (request.Request.Type is EmailType.Secondary)
            return Results.BadRequest("Cannot change a secondary phone number.");

        var currentEmail = user.Emails.FirstOrDefault(x => x.Type == request.Request.Type);
        if (currentEmail is null) return Results.BadRequest("User's primary email address is missing");

        if (options.RequireUniqueEmail)
        {
            var isTaken = await userManager.IsEmailTakenAsync(request.Request.NewEmail, cancellationToken);
            if (isTaken) return Results.BadRequest("This email address is already taken");
        }

        if (user.HasLinkedAccount())
            return Results.BadRequest("Cannot change email, first disconnect linked accounts.");

        var currentEmailVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Change, cancellationToken);

        if (!currentEmailVerificationResult.Succeeded) return currentEmailVerificationResult;

        var newEmailVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Verify, cancellationToken);

        if (!newEmailVerificationResult.Succeeded) return newEmailVerificationResult;

        var result = await userManager.ChangeEmailAsync(user, currentEmail.Email,
            request.Request.NewEmail, cancellationToken);

        return result;
    }
}