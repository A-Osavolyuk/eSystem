namespace eShop.Auth.Api.Features.Users.Queries;

internal sealed record GetUserLockoutStatusQuery(string Email) : IRequest<Result>;

internal sealed class GetUserLockoutStatusQueryHandler(
    IUserManager userManager,
    ILockoutManager lockoutManager) : IRequestHandler<GetUserLockoutStatusQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;

    public async Task<Result> Handle(GetUserLockoutStatusQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Email}.");
        }

        var lockoutStatus = await lockoutManager.GetStatusAsync(user, cancellationToken);
        
        return Result.Success(Mapper.Map(lockoutStatus));
    }
}