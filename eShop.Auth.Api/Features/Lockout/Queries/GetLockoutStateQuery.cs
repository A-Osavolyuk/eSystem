namespace eShop.Auth.Api.Features.Lockout.Queries;

public record GetLockoutStateQuery(string Email) : IRequest<Result>;

public class GetLockoutStateQueryHandler(
    ILockoutManager lockoutManager,
    IUserManager userManager) : IRequestHandler<GetLockoutStateQuery, Result>
{
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetLockoutStateQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with email {request.Email}.");
        }

        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);
        var result = Mapper.Map(lockoutState);
        
        return Result.Success(result);
    }
}