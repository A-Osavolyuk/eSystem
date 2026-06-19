using eSecurity.Idp.Security.Authorization.Verification;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.Email.Commands;

public record RemoveEmailCommand(RemoveEmailRequest Request) : IRequest<Result>;

public class RemoveEmailCommandHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    ILinkedAccountManager linkedAccountManager,
    IEmailQueryService emailQueryService,
    IEmailCommandService emailCommandService,
    IVerificationManager verificationManager) : IRequestHandler<RemoveEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly IEmailCommandService _emailCommandService = emailCommandService;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(RemoveEmailCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userManager.GetUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var email = await _emailQueryService.GetByEmailAsync(user.Id, request.Request.Email, cancellationToken);
        if (email is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.InvalidEmail,
                Description = "User doesn't owe this email."
            });
        }

        if (email.Type == EmailType.Primary)
        {
            var passkeys = await _passkeyManager.GetAllAsync(user, cancellationToken);
            if (passkeys.Count == 0)
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.InvalidEmail,
                    Description = "Cannot remove the primary email, because it is the only authentication method"
                });
            }

            if (await _linkedAccountManager.HasAsync(user, cancellationToken))
            {
                return Results.ClientError(ClientErrorCode.BadRequest, new Error
                {
                    Code = ErrorCode.LinkedAccountConnected,
                    Description = "Cannot remove the primary email, because there are one or more linked accounts"
                });
            }
        }

        var verification = await _verificationManager.FindByIdAsync(request.Request.VerificationId, cancellationToken);
        if (verification?.Status is not VerificationStatus.Approved)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.BadRequest,
                Description = "Unverified request."
            });
        }

        var verificationResult = await _verificationManager.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var result = await _emailCommandService.RemoveAsync(user.Id, request.Request.Email, cancellationToken);
        return result;
    }
}