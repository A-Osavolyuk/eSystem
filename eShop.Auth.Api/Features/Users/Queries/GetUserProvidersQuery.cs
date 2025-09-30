using eShop.Domain.Common.Security.Constants;
using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserProvidersQuery(Guid Id) : IRequest<Result>;

public class GetUserProvidersQueryHandler(IUserManager userManager) : IRequestHandler<GetUserProvidersQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserProvidersQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Id}.");

        var providers = user.TwoFactorProviders.ToList();
        var result = providers.Select(provider => new UserProviderDto()
        {
            Id = provider.Provider.Id,
            Type = provider.Provider.Type,
            UpdateDate = provider.UpdateDate,
        }).ToList();
        
        return Result.Success(result);
    }
}