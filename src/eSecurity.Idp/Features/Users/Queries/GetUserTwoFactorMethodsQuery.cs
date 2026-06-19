using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.DTOs;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users.Queries;

public record GetUserTwoFactorMethodsQuery : IRequest<Result>;

public class GetUserProvidersQueryHandler(
    ICurrentUserAccessor currentUserAccessor,
    ITwoFactorManager twoFactorManager) : IRequestHandler<GetUserTwoFactorMethodsQuery, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(GetUserTwoFactorMethodsQuery request, CancellationToken cancellationToken)
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

        var methods = await _twoFactorManager.GetAllAsync(user, cancellationToken);
        var response = methods.Select(provider => new UserTwoFactorMethod
        {
            Method = provider.Method,
            Preferred = provider.Preferred,
            UpdatedAt = provider.UpdatedAt,
        }).ToList();
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}