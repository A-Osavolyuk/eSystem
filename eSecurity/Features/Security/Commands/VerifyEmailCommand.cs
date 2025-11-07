using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Security.Commands;

public sealed record VerifyEmailCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
}

public sealed class VerifyEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Verify, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var confirmResult = await userManager.VerifyEmailAsync(user, request.Email, cancellationToken);
        if (!confirmResult.Succeeded) return confirmResult;

        return Result.Success("Your email address was successfully confirmed.");
    }
}