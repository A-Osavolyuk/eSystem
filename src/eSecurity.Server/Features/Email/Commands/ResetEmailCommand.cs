using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Email.Commands;

public record ResetEmailCommand(ResetEmailRequest Request) : IRequest<Result>;

public class ResetEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IVerificationManager verificationManager,
    IOptions<AccountOptions> options) : IRequestHandler<ResetEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(ResetEmailCommand request, CancellationToken cancellationToken)
    {
        var newEmail = request.Request.NewEmail;

        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var userCurrentEmail = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (userCurrentEmail is null)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidEmail,
                Description = "User's primary email address is missing"
            });
        }

        if (_options.RequireUniqueEmail)
        {
            var isTaken = await _emailManager.IsTakenAsync(request.Request.NewEmail, cancellationToken);
            if (isTaken)
            {
                return Results.BadRequest(new Error()
                {
                    Code = ErrorTypes.Common.EmailTaken,
                    Description = "User's primary email address is missing"
                });
            }
        }

        var resetVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Reset, cancellationToken);

        if (!resetVerificationResult.Succeeded) return resetVerificationResult;
        
        var verifyVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Verify, cancellationToken);

        if (!verifyVerificationResult.Succeeded) return verifyVerificationResult;

        var result = await _emailManager.ResetAsync(user, userCurrentEmail.Email, newEmail, cancellationToken);
        return result;
    }
}