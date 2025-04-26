namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record GetUserLockoutStatusQuery(string Email) : IRequest<Result>;

internal sealed class GetUserLockoutStatusQueryHandler(
    UserManager<UserEntity> userManager) : IRequestHandler<GetUserLockoutStatusQuery, Result>
{
    private readonly UserManager<UserEntity> userManager = userManager;

    public async Task<Result> Handle(GetUserLockoutStatusQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Email}.");
        }

        var lockoutStatus = await userManager.GetLockoutStatusAsync(user);
        
        return Result.Success(Mapper.Map(lockoutStatus));
    }
}