using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Phone;

public record RemovePhoneNumberCommand(RemovePhoneNumberRequest Request) : IRequest<Result>;

public class RemovePhoneNumberCommandHandler(
    IUserManager userManager,
    IPhoneManager phoneManager,
    ITwoFactorManager twoFactorManager,
    IVerificationManager verificationManager) : IRequestHandler<RemovePhoneNumberCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPhoneManager _phoneManager = phoneManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(RemovePhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        if (!await _phoneManager.HasAsync(user, PhoneNumberType.Primary, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPhone,
                Description = "Cannot remove phone number. Phone number is not provided."
            });
        }

        if (await _twoFactorManager.HasMethodAsync(user, TwoFactorMethod.Sms, cancellationToken))
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPhone,
                Description = "Cannot remove phone number. First disable 2FA with SMS."
            });
        }
        
        var verificationResult = await _verificationManager.VerifyAsync(user, 
            PurposeType.PhoneNumber, ActionType.Remove, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await _phoneManager.RemoveAsync(user, request.Request.PhoneNumber, cancellationToken);
        return result;
    }
}