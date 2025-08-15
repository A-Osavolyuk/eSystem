namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserPersonalQuery(Guid UserId) : IRequest<Result>;

public class GetUserPersonalQueryHandler(
    IUserManager userManager,
    IPersonalDataManager personalDataManager) : IRequestHandler<GetUserPersonalQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(GetUserPersonalQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var personalData = await personalDataManager.FindAsync(user, cancellationToken);
        if (personalData is null) return Results.NotFound($"Cannot find personal data of user with ID {request.UserId}.");
        
        var response = Mapper.Map(personalData);
        return Result.Success(response);
    }
}