using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.TwoFactor.Recovery;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Verification.Commands;

public record VerifyRecoveryCodeCommand(VerifyRecoveryCodeRequest Request) : IRequest<Result>;

public class VerifyRecoveryCodeCommandHandler(
    IUserManager userManager,
    IRecoverManager recoverManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyAuthenticatorCodeCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IRecoverManager _recoverManager = recoverManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyAuthenticatorCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User was not found");

        var codeResult = await _recoverManager.VerifyAsync(user, request.Request.Code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;
        
        return await _verificationManager.CreateAsync(user, 
            request.Request.Purpose, request.Request.Action, cancellationToken);
    }
}