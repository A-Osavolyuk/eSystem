using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authorization.Access.Codes;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;
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
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var code = request.Request.Code;
        var sender = request.Request.Sender;
        var action = request.Request.Action;
        var purpose = request.Request.Purpose;
        
        var codeResult = await _codeManager.VerifyAsync(user, code, sender, action, purpose, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        var result = await _verificationManager.CreateAsync(user, purpose, action, cancellationToken);
        return result;
    }
}