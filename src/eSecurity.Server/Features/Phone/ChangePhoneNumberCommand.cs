using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Phone;

public sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request) : IRequest<Result>;

public sealed class RequestChangePhoneNumberCommandHandler(
    IUserManager userManager,
    IPhoneManager phoneManager,
    IVerificationManager verificationManager,
    IOptions<AccountOptions> options) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPhoneManager _phoneManager = phoneManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(ChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        if (request.Request.Type is PhoneNumberType.Secondary)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidPhone,
                Description = "Cannot change a secondary phone number."
            });
        }

        var phoneNumber = await _phoneManager.FindByTypeAsync(user, request.Request.Type, cancellationToken);
        if (phoneNumber is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidPhone,
                Description = "User's phone number is missing."
            });
        }

        if (_options.RequireUniquePhoneNumber)
        {
            var isTaken = await _phoneManager.IsTakenAsync(request.Request.NewPhoneNumber, cancellationToken);
            if (isTaken)
            {
                return Results.BadRequest(new Error()
                {
                    Code = Errors.Common.PhoneTaken,
                    Description = "This phone number is already taken"
                });
            }
        }

        var currentPhoneNumberVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Change, cancellationToken);

        if (!currentPhoneNumberVerificationResult.Succeeded) return currentPhoneNumberVerificationResult;

        var newPhoneNumberVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Verify, cancellationToken);

        if (!newPhoneNumberVerificationResult.Succeeded) return newPhoneNumberVerificationResult;

        var result = await _phoneManager.ChangeAsync(user, phoneNumber.PhoneNumber,
            request.Request.NewPhoneNumber, cancellationToken);

        return result;
    }
}