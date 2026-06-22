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
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var response = await _recoverManager.UnprotectAsync(user, cancellationToken);
        return Results.Success(SuccessCodes.Ok, response);
    }
}