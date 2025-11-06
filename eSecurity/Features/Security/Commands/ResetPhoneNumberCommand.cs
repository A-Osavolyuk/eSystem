using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.Options;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSecurity.Features.Security.Commands;

public record ResetPhoneNumberCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string NewPhoneNumber { get; set; }
}

public class ResetPhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IOptions<AccountOptions> options) : IRequestHandler<ResetPhoneNumberCommand, Result>
{
    private readonly AccountOptions options = options.Value;
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(ResetPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);

        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}");
        
        var userCurrentPhoneNumber = user.GetPhoneNumber(PhoneNumberType.Primary);
        if (userCurrentPhoneNumber is null) return Results.BadRequest("User's primary phone number is missing");
        
        if (options.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.NewPhoneNumber, cancellationToken);
            if (isTaken) return Results.BadRequest("This phone number is already taken");
        }
        
        var resetVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Reset, cancellationToken);

        if (!resetVerificationResult.Succeeded) return resetVerificationResult;
        
        var verifyVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Verify, cancellationToken);

        if (!verifyVerificationResult.Succeeded) return verifyVerificationResult;
        
        var newPhoneNumber = request.NewPhoneNumber;
        
        var result = await userManager.ResetPhoneNumberAsync(user, 
            userCurrentPhoneNumber.PhoneNumber, newPhoneNumber, cancellationToken);
        
        return result;
    }
}