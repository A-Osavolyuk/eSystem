using eSecurity.Core.DTOs;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public record GetUserTwoFactorMethodsQuery : IRequest<Result>;

public class GetUserProvidersQueryHandler(
    ICurrentUserAccessor currentUserAccessor,
    ITwoFactorQueryService twoFactorQueryService) : IRequestHandler<GetUserTwoFactorMethodsQuery, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly ITwoFactorQueryService _twoFactorQueryService = twoFactorQueryService;

    public async Task<Result> Handle(GetUserTwoFactorMethodsQuery request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var methods = await _twoFactorQueryService.ListByUserAsync(user.Id, cancellationToken);
        var response = methods.Select(provider => new UserTwoFactorMethod
        {
            Method = provider.Method,
            Preferred = provider.Preferred,
            UpdatedAt = provider.UpdatedAt,
        }).ToList();
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}