using eShop.Auth.Api.Security.Authentication.TwoFactor.Authenticator;
using eShop.Domain.Common.Results;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Verification.Commands;

public record VerifyAuthenticatorCodeCommand(VerifyAuthenticatorCodeRequest Request) : IRequest<Result>;

public class VerifyAuthenticatorCodeCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyAuthenticatorCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyAuthenticatorCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var userSecret = await secretManager.FindAsync(user, cancellationToken);
        if (userSecret is null) return Results.NotFound("Not found user secret");

        var unprotectedSecret = secretManager.Unprotect(userSecret.Secret);
        var verified = AuthenticatorUtils.VerifyCode(request.Request.Code, unprotectedSecret);
        if (!verified) return Results.BadRequest("Invalid authenticator code");
        
        var purpose = request.Request.Purpose;
        var action = request.Request.Action;
        var result = await verificationManager.CreateAsync(user, purpose, action, cancellationToken);
        return result;
    }
}