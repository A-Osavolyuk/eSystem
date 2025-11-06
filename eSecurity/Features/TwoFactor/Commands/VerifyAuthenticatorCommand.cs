using eSecurity.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Features.TwoFactor.Commands;

public record VerifyAuthenticatorCommand() : IRequest<Result>
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
    public required string Secret { get; set; }
}

public class VerifyAuthenticatorCommandHandler(
    IUserManager userManager) : IRequestHandler<VerifyAuthenticatorCommand, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(VerifyAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var verified = AuthenticatorUtils.VerifyCode(request.Code, request.Secret);
        return verified ? Result.Success() : Results.BadRequest("Invalid code.");
    }
}