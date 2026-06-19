using eSecurity.Idp.Security.Authentication.TwoFactor.RecoveryCode;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.TwoFactor.Commands;

public record GenerateRecoveryCodesCommand() : IRequest<Result>;

public class GenerateRecoveryCodesCommandHandler(
    IRecoverManager recoverManager,
    ICurrentUserAccessor currentUserAccessor) : IRequestHandler<GenerateRecoveryCodesCommand, Result>
{
    private readonly IRecoverManager _recoverManager = recoverManager;
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;

    public async Task<Result> Handle(GenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
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

        var response = await _recoverManager.GenerateAsync(user, cancellationToken);
        return Results.Success(SuccessCodes.Ok, response);
    }
}