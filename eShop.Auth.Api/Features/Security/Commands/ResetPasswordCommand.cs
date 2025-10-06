using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<Result>;

public sealed class ResetPasswordCommandHandler(
    IUserManager userManager,
    IVerificationManager verificationManager) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        if (!user.HasPassword()) return Results.BadRequest("User does not have a password.");
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.Password, CodeType.Reset, cancellationToken);
        
        if(!verificationResult.Succeeded)  return verificationResult;
        
        var result = await userManager.ResetPasswordAsync(user, request.Request.NewPassword, cancellationToken);
        return result;
    }
}