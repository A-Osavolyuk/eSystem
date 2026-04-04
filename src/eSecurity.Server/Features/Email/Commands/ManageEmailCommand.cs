using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Security.Authorization.Verification;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Email.Commands;

public record ManageEmailCommand(ManageEmailRequest Request) : IRequest<Result>;

public class ManageEmailCommandHandler(
    IUserManager userManager,
    IEmailManager emailManager,
    IVerificationManager verificationManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<ManageEmailCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(ManageEmailCommand request, CancellationToken cancellationToken)
    {
        var type = request.Request.Type;
        var email = request.Request.Email;
        
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var verification = await _verificationManager.FindByIdAsync(request.Request.VerificationId, cancellationToken);
        if (verification?.Status is not VerificationStatus.Approved) 
            return Results.BadRequest("Unverified request.");

        var verificationResult = await _verificationManager.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var result = await _emailManager.ManageAsync(user, type, email, cancellationToken);
        return result;
    }
}