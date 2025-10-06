using eShop.Auth.Api.Security.Protection;
using eShop.Domain.Requests.Auth;
using OtpNet;

namespace eShop.Auth.Api.Features.Verification.Commands;

public record VerifyAuthenticatorCodeCommand(VerifyAuthenticatorCodeRequest Request) : IRequest<Result>;

public class VerifyAuthenticatorCodeCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager,
    IVerificationManager verificationManager,
    SecretProtector secretProtector) : IRequestHandler<VerifyAuthenticatorCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly IVerificationManager verificationManager = verificationManager;
    private readonly SecretProtector secretProtector = secretProtector;

    public async Task<Result> Handle(VerifyAuthenticatorCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var userSecret = await secretManager.FindAsync(user, cancellationToken);
        if (userSecret is null) return Results.NotFound("Not found user secret");

        var unprotectedSecret = secretProtector.Unprotect(userSecret.Secret);
        var verified = AuthenticatorUtils.VerifyCode(request.Request.Code, unprotectedSecret);
        if (!verified) return Results.BadRequest("Invalid authenticator code");
        
        var resource = request.Request.Resource;
        var type = request.Request.Type;
        var result = await verificationManager.CreateAsync(user, resource, type, cancellationToken);
        return result;
    }
}