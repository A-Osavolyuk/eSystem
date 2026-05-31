using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.Requests;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Idp.Features.TwoFactor.Commands;

public record PreferTwoFactorMethodCommand(PreferTwoFactorMethodRequest Request) : IRequest<Result>;

public class PreferMethodCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager) : IRequestHandler<PreferTwoFactorMethodCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(PreferTwoFactorMethodCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userManager.GetUserAsync(cancellationToken);
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

        var result = await _twoFactorManager.PreferAsync(user, request.Request.PreferredMethod, cancellationToken);
        return result;
    }
}