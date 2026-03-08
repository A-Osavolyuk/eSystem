using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authorization;
using eSecurity.Server.Security.Authorization.Codes;
using eSecurity.Server.Security.Authorization.Verification;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Verification.Commands;

public record VerifyCodeCommand(VerifyCodeRequest Request) : IRequest<Result>;

public class VerifyCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IVerificationManager verificationManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<VerifyCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request.");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var code = await _codeManager.FindAsync(user, request.Request.Code, cancellationToken);
        if (code is null) return Results.NotFound("Code not found.");

        var codeResult = await _codeManager.RemoveAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        return await _verificationManager.CreateAsync(user, code.Purpose, code.Action, cancellationToken);
    }
}