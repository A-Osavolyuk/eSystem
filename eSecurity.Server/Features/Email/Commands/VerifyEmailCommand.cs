using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Email.Commands;

public sealed record VerifyEmailCommand(VerifyEmailRequest Request) : IRequest<Result>;

public sealed class VerifyEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Verify, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var confirmResult = await _emailManager.VerifyAsync(user, request.Request.Email, cancellationToken);
        if (!confirmResult.Succeeded) return confirmResult;

        return Results.Ok("Your email address was successfully confirmed.");
    }
}