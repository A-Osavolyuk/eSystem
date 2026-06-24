using eSecurity.Idp.Security.Authentication.TwoFactor.RecoveryCode;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.TwoFactor;

public record LoadRecoveryCodesCommand : IRequest<Result>;

public class LoadRecoveryCodesCommandHandler(
    ICurrentUserAccessor currentUserAccessor,
    IRecoverManager recoverManager) : IRequestHandler<LoadRecoveryCodesCommand, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IRecoverManager _recoverManager = recoverManager;
    
    public async Task<Result> Handle(LoadRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var response = await _recoverManager.UnprotectAsync(user, cancellationToken);
        return Results.Success(SuccessCodes.Ok, response);
    }
}