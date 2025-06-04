namespace eShop.Auth.Api.Features.Users.Queries;

public record GetPersonalDataQuery(Guid UserId) : IRequest<Result>;

public class GetPersonalDataQueryHandler(
    IUserManager userManager, 
    IPersonalDataManager personalDataManager) : IRequestHandler<GetPersonalDataQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IPersonalDataManager personalDataManager = personalDataManager;

    public async Task<Result> Handle(GetPersonalDataQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound("User not found");
        }

        var personalData = await personalDataManager.FindAsync(user, cancellationToken);

        if (personalData is null)
        {
            return Results.NotFound("Personal data not found");
        }

        var response = Mapper.Map(personalData);
        
        return Result.Success(response);
    }
}