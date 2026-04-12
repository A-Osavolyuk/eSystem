using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserTwoFactorMethodsQuery : IRequest<Result>;

public class GetUserProvidersQueryHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserTwoFactorMethodsQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserTwoFactorMethodsQuery request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid subject."
            });
        }
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found."
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