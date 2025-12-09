using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.TwoFactor.Recovery;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record LoadRecoveryCodesCommand(LoadRecoveryCodesRequest Request) : IRequest<Result>;

public class LoadRecoveryCodesCommandHandler(
    IUserManager userManager,
    IRecoverManager recoverManager) : IRequestHandler<LoadRecoveryCodesCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IRecoverManager _recoverManager = recoverManager;
    
    public async Task<Result> Handle(LoadRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var codes = _recoverManager.UnprotectAsync(user);
        return Results.Ok(codes);
    }
}