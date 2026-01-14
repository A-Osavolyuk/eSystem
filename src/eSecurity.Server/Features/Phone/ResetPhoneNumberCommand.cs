using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.Phone;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Phone;

public record ResetPhoneNumberCommand(ResetPhoneNumberRequest Request) : IRequest<Result>;

public class ResetPhoneNumberCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager,
    IPhoneManager phoneManager,
    IOptions<AccountOptions> options) : IRequestHandler<ResetPhoneNumberCommand, Result>
{
    private readonly AccountOptions _options = options.Value;
    private readonly IUserManager _userManager = userManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly IPhoneManager _phoneManager = phoneManager;

    public async Task<Result> Handle(ResetPhoneNumberCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var phoneNumber = await _phoneManager.FindByTypeAsync(user, PhoneNumberType.Primary, cancellationToken);
        if (phoneNumber is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidPhone,
                Description = "User's primary phone number is missing"
            });
        }
        
        if (_options.RequireUniquePhoneNumber)
        {
            var isTaken = await _phoneManager.IsTakenAsync(request.Request.NewPhoneNumber, cancellationToken);
            if (isTaken)
            {
                return Results.BadRequest(new Error()
                {
                    Code = ErrorTypes.Common.PhoneTaken,
                    Description = "This phone number is already taken"
                });
            }
        }
        
        var resetVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Reset, cancellationToken);

        if (!resetVerificationResult.Succeeded) return resetVerificationResult;
        
        var verifyVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.PhoneNumber, ActionType.Verify, cancellationToken);

        if (!verifyVerificationResult.Succeeded) return verifyVerificationResult;
        
        var newPhoneNumber = request.Request.NewPhoneNumber;
        
        var result = await _phoneManager.ResetAsync(user, 
            phoneNumber.PhoneNumber, newPhoneNumber, cancellationToken);
        
        return result;
    }
}