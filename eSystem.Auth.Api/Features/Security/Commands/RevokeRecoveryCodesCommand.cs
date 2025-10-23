using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Requests.Auth;

namespace eSystem.Auth.Api.Features.Security.Commands;

public record RevokeRecoveryCodesCommand(RevokeRecoveryCodesRequest Request) : IRequest<Result>;

public class RevokeRecoveryCodesCommandHandler(
    IUserManager userManager,
    IRecoverManager recoverManager) : IRequestHandler<RevokeRecoveryCodesCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRecoverManager recoverManager = recoverManager;

    public async Task<Result> Handle(RevokeRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        if (!user.HasRecoveryCodes()) return Results.BadRequest("User doesn't have recovery codes.");
        
        var result = await recoverManager.RevokeAsync(user, cancellationToken);
        return result;
    }
}