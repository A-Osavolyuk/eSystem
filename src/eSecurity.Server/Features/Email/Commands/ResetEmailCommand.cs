using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization;
using eSecurity.Server.Security.Authorization.Verification;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives.Constants;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Email.Commands;

public record ResetEmailCommand(ResetEmailRequest Request) : IRequest<Result>;

public class ResetEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IVerificationManager verificationManager,
    IHttpContextAccessor httpContextAccessor,
    IOptions<AccountOptions> options) : IRequestHandler<ResetEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly AccountOptions _options = options.Value;

    public async Task<Result> Handle(ResetEmailCommand request, CancellationToken cancellationToken)
    {
        var newEmail = request.Request.Email;

        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var userCurrentEmail = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (userCurrentEmail is null)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidEmail,
                Description = "User's primary email address is missing"
            });
        }

        if (_options.RequireUniqueEmail)
        {
            var isTaken = await _emailManager.IsTakenAsync(request.Request.Email, cancellationToken);
            if (isTaken)
            {
                return Results.BadRequest(new Error
                {
                    Code = ErrorTypes.Common.EmailTaken,
                    Description = "User's primary email address is missing"
                });
            }
        }

        var verification = await _verificationManager.FindByIdAsync(request.Request.VerificationId, cancellationToken);
        if (verification?.Status is not VerificationStatus.Approved) 
            return Results.BadRequest("Unverified request.");

        var verificationResult = await _verificationManager.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var result = await _emailManager.ResetAsync(user, userCurrentEmail.Email, newEmail, cancellationToken);
        return result;
    }
}