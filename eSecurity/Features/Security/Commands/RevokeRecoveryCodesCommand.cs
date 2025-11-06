using eSecurity.Security.Authentication.TwoFactor.Recovery;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Security.Commands;

public record RevokeRecoveryCodesCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
}

public class RevokeRecoveryCodesCommandHandler(
    IUserManager userManager,
    IRecoverManager recoverManager) : IRequestHandler<RevokeRecoveryCodesCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRecoverManager recoverManager = recoverManager;

    public async Task<Result> Handle(RevokeRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        if (!user.HasRecoveryCodes()) return Results.BadRequest("User doesn't have recovery codes.");
        
        var result = await recoverManager.RevokeAsync(user, cancellationToken);
        return result;
    }
}