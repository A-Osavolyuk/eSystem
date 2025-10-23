using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Requests.Auth;
using eSystem.Domain.Security.Verification;

namespace eSystem.Auth.Api.Features.Security.Commands;

public sealed record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest<Result>;

public sealed class VerifyEmailCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Verify, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var confirmResult = await userManager.VerifyEmailAsync(user, request.Request.Email, cancellationToken);
        if (!confirmResult.Succeeded) return confirmResult;

        return Result.Success("Your email address was successfully confirmed.");
    }
}