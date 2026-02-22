using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authorization;
using eSecurity.Server.Security.Authorization.Codes;
using eSecurity.Server.Security.Authorization.Verification;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Verification.Commands;

public record VerifyCodeCommand(VerifyCodeRequest Request) : IRequest<Result>;

public class VerifyCodeCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindBySubjectAsync(request.Request.Subject, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var code = await _codeManager.FindAsync(user, request.Request.Code, cancellationToken);
        if (code is null) return Results.NotFound("Code not found.");

        var codeResult = await _codeManager.RemoveAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        return await _verificationManager.CreateAsync(user, code.Purpose, code.Action, cancellationToken);
    }
}