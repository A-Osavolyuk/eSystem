namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserLockoutQuery(Guid Id) : IRequest<Result>;

public class GetLockoutStateQueryHandler(
    ILockoutManager lockoutManager,
    IUserManager userManager) : IRequestHandler<GetUserLockoutQuery, Result>
{
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserLockoutQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Id}.");
        
        var response = Mapper.Map(user.LockoutState);
        
        return Result.Success(response);
    }
}