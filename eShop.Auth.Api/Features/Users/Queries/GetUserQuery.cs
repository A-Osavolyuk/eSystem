namespace eShop.Auth.Api.Features.Users.Queries;

public sealed record GetUserQuery(Guid UserId) : IRequest<Result>;

public sealed class GetUserQueryHandler(
    IPersonalDataManager personalDataManager,
    IUserManager userManager) : IRequestHandler<GetUserQuery, Result>
{
    private readonly IPersonalDataManager personalDataManager = personalDataManager;
    private readonly IUserManager userManager = userManager;


    public async Task<Result> Handle(GetUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        }

        var response = Mapper.Map(user);
        
        var personalData = await personalDataManager.FindAsync(user, cancellationToken);

        if (personalData is not null)
        {
            response.PersonalData = Mapper.Map(personalData);
        }
        
        return Result.Success(response);
    }
}