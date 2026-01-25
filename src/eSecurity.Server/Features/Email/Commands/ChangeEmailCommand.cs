using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Email.Commands;

public sealed record ChangeEmailCommand(ChangeEmailRequest Request) : IRequest<Result>;

public sealed class RequestChangeEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    ILinkedAccountManager linkedAccountManager,
    IVerificationManager verificationManager,
    IOptions<AccountOptions> options) : IRequestHandler<ChangeEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(ChangeEmailCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        if (request.Request.Type is EmailType.Secondary)
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidEmail,
                Description = "Cannot change a secondary phone number."
            });

        var currentEmail = await _emailManager.FindByTypeAsync(user, request.Request.Type, cancellationToken);
        if (currentEmail is null)
        {
            return Results.BadRequest(new Error
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
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.Common.EmailTaken,
                    Description = "Email address is already taken"
                });
            }
        }

        if (await _linkedAccountManager.HasAsync(user, cancellationToken))
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.LinkedAccountConnected,
                Description = "Cannot change email, first disconnect linked accounts."
            });

        var currentEmailVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Change, cancellationToken);

        if (!currentEmailVerificationResult.Succeeded) return currentEmailVerificationResult;

        var newEmailVerificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Email, ActionType.Verify, cancellationToken);

        if (!newEmailVerificationResult.Succeeded) return newEmailVerificationResult;

        var result = await _emailManager.ChangeAsync(user, currentEmail.Email,
            request.Request.NewEmail, cancellationToken);

        return result;
    }
}