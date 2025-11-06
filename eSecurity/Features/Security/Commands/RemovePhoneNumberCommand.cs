using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Authentication.TwoFactor;
using eSystem.Core.Security.Authorization.Access;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSecurity.Features.Security.Commands;

public record RemovePhoneNumberCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string PhoneNumber { get; set; }
}

public class RemovePhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<RemovePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(RemovePhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        if (!user.HasPhoneNumber(PhoneNumberType.Primary)) return Results.BadRequest(
            "Cannot remove phone number. Phone number is not provided.");

        if (user.HasTwoFactor(TwoFactorMethod.Sms))
            return Results.BadRequest("Cannot remove phone number. First disable 2FA with SMS.");
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            PurposeType.PhoneNumber, ActionType.Remove, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await userManager.RemovePhoneNumberAsync(user, request.PhoneNumber, cancellationToken);
        return result;
    }
}