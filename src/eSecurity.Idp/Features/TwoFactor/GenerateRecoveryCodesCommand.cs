using eSecurity.Idp.Security.Authentication.TwoFactor.RecoveryCode;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.TwoFactor;

public record GenerateRecoveryCodesCommand : IRequest<Result>;

public class GenerateRecoveryCodesCommandHandler(
    IRecoverManager recoverManager,
    ICurrentUserAccessor currentUserAccessor) : IRequestHandler<GenerateRecoveryCodesCommand, Result>
{
    private readonly IRecoverManager _recoverManager = recoverManager;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;

    public async Task<Result> Handle(GenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var response = await _recoverManager.GenerateAsync(user, cancellationToken);
        return Results.Success(SuccessCodes.Ok, response);
    }
}