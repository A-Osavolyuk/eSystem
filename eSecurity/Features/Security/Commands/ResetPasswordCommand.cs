using eSecurity.Security.Authentication.Password;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Features.Security.Commands;

public sealed record ResetPasswordCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; } = string.Empty;
}

public sealed class ResetPasswordCommandHandler(
    IUserManager userManager,
    IPasswordManager passwordManager,
    IVerificationManager verificationManager) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasswordManager passwordManager = passwordManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        if (!user.HasPassword()) return Results.BadRequest("User does not have a password.");
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            PurposeType.Password, ActionType.Reset, cancellationToken);
        
        if(!verificationResult.Succeeded)  return verificationResult;
        
        var result = await passwordManager.ResetAsync(user, request.NewPassword, cancellationToken);
        return result;
    }
}