using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.TwoFactor.Recovery;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record GenerateRecoveryCodesCommand(GenerateRecoveryCodesRequest Request) : IRequest<Result>;

public class GenerateRecoveryCodesCommandHandler(
    IRecoverManager recoverManager,
    IUserManager userManager) : IRequestHandler<GenerateRecoveryCodesCommand, Result>
{
    private readonly IRecoverManager _recoverManager = recoverManager;
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(GenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        var response = await _recoverManager.GenerateAsync(user, cancellationToken);
        return Results.Ok(response);
    }
}