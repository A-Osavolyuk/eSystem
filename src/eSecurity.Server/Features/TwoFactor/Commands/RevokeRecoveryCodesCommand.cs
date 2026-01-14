using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.TwoFactor.Recovery;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record RevokeRecoveryCodesCommand(RevokeRecoveryCodesRequest Request) : IRequest<Result>;
public class RevokeRecoveryCodesCommandHandler(
    IUserManager userManager,
    IRecoverManager recoverManager) : IRequestHandler<RevokeRecoveryCodesCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IRecoverManager _recoverManager = recoverManager;

    public async Task<Result> Handle(RevokeRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        if (await _recoverManager.HasAsync(user, cancellationToken)) 
            return Results.BadRequest("User doesn't have recovery codes.");
        
        var result = await _recoverManager.RevokeAsync(user, cancellationToken);
        return result;
    }
}