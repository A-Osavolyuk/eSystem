using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Users.Queries;

public sealed record GetUserQuery(Guid UserId) : IRequest<Result>;

public sealed class GetUserQueryHandler(
    IUserManager userManager) : IRequestHandler<GetUserQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    
    public async Task<Result> Handle(GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = Mapper.Map(user);
        return Result.Success(response);
    }
}