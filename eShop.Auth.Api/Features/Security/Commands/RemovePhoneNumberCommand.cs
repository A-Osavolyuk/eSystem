using eShop.Domain.Common.Security.Constants;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record RemovePhoneNumberCommand(RemovePhoneNumberRequest Request) :  IRequest<Result>;

public class RemovePhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<RemovePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(RemovePhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (!user.HasPhoneNumber(PhoneNumberType.Primary)) return Results.BadRequest(
            "Cannot remove phone number. Phone number is not provided.");

        if (user.HasTwoFactor(TwoFactorMethod.Sms))
            return Results.BadRequest("Cannot remove phone number. First disable 2FA with SMS.");
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.PhoneNumber, CodeType.Remove, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await userManager.RemovePhoneNumberAsync(user, request.Request.PhoneNumber, cancellationToken);
        return result;
    }
}