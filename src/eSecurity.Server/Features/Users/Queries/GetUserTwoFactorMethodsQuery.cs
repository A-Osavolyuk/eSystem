using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserTwoFactorMethodsQuery(Guid Id) : IRequest<Result>;

public class GetUserProvidersQueryHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager) : IRequestHandler<GetUserTwoFactorMethodsQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;

    public async Task<Result> Handle(GetUserTwoFactorMethodsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Id, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var methods = await _twoFactorManager.GetAllAsync(user, cancellationToken);
        var response = methods.Select(provider => new UserTwoFactorMethod
        {
            Method = provider.Method,
            Preferred = provider.Preferred,
            UpdateDate = provider.UpdateDate,
        }).ToList();
        
        return Results.Ok(response);
    }
}