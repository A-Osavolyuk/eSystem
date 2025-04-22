using eShop.Domain.Common.API;

namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record GetUserLockoutStatusQuery(string Email) : IRequest<Result>;

internal sealed class GetUserLockoutStatusQueryHandler(
    AppManager appManager) : IRequestHandler<GetUserLockoutStatusQuery, Result>
{
    private readonly AppManager appManager = appManager;

    public async Task<Result> Handle(GetUserLockoutStatusQuery request,
        CancellationToken cancellationToken)
    {
        var user = await appManager.UserManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Email}.");
        }

        var lockoutStatus = await appManager.UserManager.GetLockoutStatusAsync(user);
        
        return Result.Success(Mapper.Map(lockoutStatus));
    }
}