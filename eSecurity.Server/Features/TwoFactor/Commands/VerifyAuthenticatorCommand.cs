using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record VerifyAuthenticatorCommand(VerifyAuthenticatorRequest Request) : IRequest<Result>;

public class VerifyAuthenticatorCommandHandler(
    IUserManager userManager) : IRequestHandler<VerifyAuthenticatorCommand, Result>
{
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(VerifyAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var verified = AuthenticatorUtils.VerifyCode(request.Request.Code, request.Request.Secret);
        return verified ? Results.Ok() : Results.BadRequest("Invalid code.");
    }
}