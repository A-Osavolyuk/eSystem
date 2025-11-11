using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Phone;

public record ResetPhoneNumberCommand(ResetPhoneNumberRequest Request) : IRequest<Result>;

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
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
        var userCurrentPhoneNumber = user.GetPhoneNumber(PhoneNumberType.Primary);
        if (userCurrentPhoneNumber is null) return Results.BadRequest("User's primary phone number is missing");
        
        if (options.RequireUniquePhoneNumber)
        {
            var isTaken = await userManager.IsPhoneNumberTakenAsync(request.Request.NewPhoneNumber, cancellationToken);
            if (isTaken) return Results.BadRequest("This phone number is already taken");
        }
        
        var resetVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Reset, cancellationToken);

        if (!resetVerificationResult.Succeeded) return resetVerificationResult;
        
        var verifyVerificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Verify, cancellationToken);

        if (!verifyVerificationResult.Succeeded) return verifyVerificationResult;
        
        var newPhoneNumber = request.Request.NewPhoneNumber;
        
        var result = await userManager.ResetPhoneNumberAsync(user, 
            userCurrentPhoneNumber.PhoneNumber, newPhoneNumber, cancellationToken);
        
        return result;
    }
}