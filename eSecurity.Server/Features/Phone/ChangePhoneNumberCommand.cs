using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Phone;

public sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request) : IRequest<Result>;

public sealed class RequestChangePhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IOptions<AccountOptions> options) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(ChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (request.Request.Type is PhoneNumberType.Secondary)
            return Results.BadRequest("Cannot change a secondary phone number.");

        var userPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.Type == request.Request.Type);
        if (userPhoneNumber is null) return Results.BadRequest("User's phone number is missing.");

        if (_options.RequireUniquePhoneNumber)
        {
            var isTaken = await _userManager.IsPhoneNumberTakenAsync(request.Request.NewPhoneNumber, cancellationToken);
            if (isTaken) return Results.BadRequest("This phone number is already taken");
        }

        var currentPhoneNumberVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Change, cancellationToken);

        if (!currentPhoneNumberVerificationResult.Succeeded) return currentPhoneNumberVerificationResult;

        var newPhoneNumberVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Verify, cancellationToken);

        if (!newPhoneNumberVerificationResult.Succeeded) return newPhoneNumberVerificationResult;

        var result = await _userManager.ChangePhoneNumberAsync(user, userPhoneNumber.PhoneNumber,
            request.Request.NewPhoneNumber, cancellationToken);

        return result;
    }
}