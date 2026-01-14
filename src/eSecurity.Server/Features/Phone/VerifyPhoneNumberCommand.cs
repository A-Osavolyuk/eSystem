using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Phone;

public sealed record VerifyPhoneNumberCommand(VerifyPhoneNumberRequest Request) : IRequest<Result>;

public sealed class VerifyPhoneNumberCommandHandler(
    IUserManager userManager,
    IPhoneManager phoneManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyPhoneNumberCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPhoneManager _phoneManager = phoneManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyPhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Verify, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await _phoneManager.VerifyAsync(user, request.Request.PhoneNumber, cancellationToken);
        return result;
    }
}