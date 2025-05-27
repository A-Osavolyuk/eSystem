namespace eShop.Auth.Api.Features.TwoFactor.Queries;

public record GetUserProvidersQuery(Guid Id) : IRequest<Result>;

public class GetUserProvidersQueryHandler(
    IProviderManager providerManager,
    IUserManager userManager) : IRequestHandler<GetUserProvidersQuery, Result>
{
    private readonly IProviderManager providerManager = providerManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserProvidersQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Id}.");
        }

        var providers = await providerManager.GetProvidersAsync(user, cancellationToken);
        var result = providers.Select(Mapper.Map).ToList();
        return Result.Success(result);
    }
}