using eSecurity.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Security.Authentication.TwoFactor.Secret;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Features.Verification.Commands;

public record VerifyAuthenticatorCodeCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
    public required PurposeType Purpose { get; set; }
    public required ActionType Action { get; set; }
}

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
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var userSecret = await secretManager.FindAsync(user, cancellationToken);
        if (userSecret is null) return Results.NotFound("Not found user secret");

        var unprotectedSecret = secretManager.Unprotect(userSecret.Secret);
        var verified = AuthenticatorUtils.VerifyCode(request.Code, unprotectedSecret);
        if (!verified) return Results.BadRequest("Invalid authenticator code");
        
        var purpose = request.Purpose;
        var action = request.Action;
        var result = await verificationManager.CreateAsync(user, purpose, action, cancellationToken);
        return result;
    }
}