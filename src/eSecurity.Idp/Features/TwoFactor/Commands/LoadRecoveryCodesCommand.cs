using eSecurity.Idp.Security.Authentication.TwoFactor.RecoveryCode;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.TwoFactor.Commands;

public record LoadRecoveryCodesCommand() : IRequest<Result>;

public class LoadRecoveryCodesCommandHandler(
    IUserQueryService userQueryService,
    ICurrentUserAccessor currentUserAccessor,
    IRecoverManager recoverManager) : IRequestHandler<LoadRecoveryCodesCommand, Result>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
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