using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

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
            CodeResource.Email, CodeType.Verify, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var confirmResult = await userManager.ConfirmEmailAsync(user, cancellationToken);
        if (!confirmResult.Succeeded) return confirmResult;

        return Result.Success("Your email address was successfully confirmed.");
    }
}