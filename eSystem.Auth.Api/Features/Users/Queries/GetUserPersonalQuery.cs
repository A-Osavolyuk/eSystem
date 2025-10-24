namespace eSystem.Auth.Api.Features.Users.Queries;

public record GetUserPersonalQuery(Guid UserId) : IRequest<Result>;

public class GetUserPersonalQueryHandler(IUserManager userManager) : IRequestHandler<GetUserPersonalQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserPersonalQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        if (user.PersonalData is null) return Results.NotFound(
            $"Cannot find personal data of user with ID {request.UserId}.");
        
        var response = Mapper.Map(user.PersonalData);
        return Result.Success(response);
    }
}