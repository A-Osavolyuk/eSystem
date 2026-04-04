using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Authorization.Verification;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Email.Commands;

public record RemoveEmailCommand(RemoveEmailRequest Request) : IRequest<Result>;

public class RemoveEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IPasskeyManager passkeyManager,
    ILinkedAccountManager linkedAccountManager,
    IVerificationManager verificationManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<RemoveEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(RemoveEmailCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var email = await _emailManager.FindByEmailAsync(user, request.Request.Email, cancellationToken);
        if (email is null)
        {
            return Results.NotFound(new Error
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
                return Results.BadRequest(new Error
                {
                    Code = ErrorCode.InvalidEmail,
                    Description = "Cannot remove the primary email, because it is the only authentication method"
                });
            }

            if (await _linkedAccountManager.HasAsync(user, cancellationToken))
            {
                return Results.BadRequest(new Error
                {
                    Code = ErrorCode.LinkedAccountConnected,
                    Description = "Cannot remove the primary email, because there are one or more linked accounts"
                });
            }
        }

        var verification = await _verificationManager.FindByIdAsync(request.Request.VerificationId, cancellationToken);
        if (verification?.Status is not VerificationStatus.Approved) 
            return Results.BadRequest("Unverified request.");

        var verificationResult = await _verificationManager.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var result = await _emailManager.RemoveAsync(user, request.Request.Email, cancellationToken);
        return result;
    }
}