using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.Options;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSecurity.Features.Security.Commands;

public sealed record ChangePhoneNumberCommand(ChangePhoneNumberRequest Request) : IRequest<Result>;

public sealed class RequestChangePhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IOptions<AccountOptions> options) : IRequestHandler<ChangePhoneNumberCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly AccountOptions options = options.Value;

    public async Task<Result> Handle(ChangePhoneNumberCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        if (request.Request.Type is PhoneNumberType.Secondary)
            return Results.BadRequest("Cannot change a secondary phone number.");

        var userPhoneNumber = user.PhoneNumbers.FirstOrDefault(x => x.Type == request.Request.Type);
        if (userPhoneNumber is null) return Results.BadRequest("User's phone number is missing.");

        if (options.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.NewPhoneNumber, cancellationToken);
            if (isTaken) return Results.BadRequest("This phone number is already taken");
        }

        var currentPhoneNumberVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Change, cancellationToken);

        if (!currentPhoneNumberVerificationResult.Succeeded) return currentPhoneNumberVerificationResult;

        var newPhoneNumberVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Verify, cancellationToken);

        if (!newPhoneNumberVerificationResult.Succeeded) return newPhoneNumberVerificationResult;

        var result = await userManager.ChangePhoneNumberAsync(user, userPhoneNumber.PhoneNumber,
            request.Request.NewPhoneNumber, cancellationToken);

        return result;
    }
}