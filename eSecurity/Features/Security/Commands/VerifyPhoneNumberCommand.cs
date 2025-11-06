using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Features.Security.Commands;

public sealed record VerifyPhoneNumberCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}

public sealed class VerifyPhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyPhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID ${request.UserId}");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Verify, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await userManager.VerifyPhoneNumberAsync(user, request.PhoneNumber, cancellationToken);
        return result;
    }
}