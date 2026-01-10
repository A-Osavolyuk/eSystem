using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Password.Commands;

public sealed record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<Result>;

public sealed class ResetPasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager,
    IVerificationManager verificationManager) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        if (!await _passwordManager.HasAsync(user, cancellationToken)) 
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidPassword,
                Description = "User does not have a password."
            });
        
        var verificationResult = await _verificationManager.VerifyAsync(user, 
            PurposeType.Password, ActionType.Reset, cancellationToken);
        
        if(!verificationResult.Succeeded)  return verificationResult;
        
        var result = await _passwordManager.ResetAsync(user, request.Request.NewPassword, cancellationToken);
        return result;
    }
}