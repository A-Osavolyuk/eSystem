using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserTwoFactorDataQuery(Guid UserId) : IRequest<Result>;

public class GetUserTwoFactorDataQueryHandler(IUserManager userManager) : IRequestHandler<GetUserTwoFactorDataQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserTwoFactorDataQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = new UserTwoFactorDto()
        {
            TwoFactorEnabled = user.TwoFactorEnabled,
            Providers = user.TwoFactorProviders.Select(x => new UserProviderDto()
            {
                Id = x.Provider.Id,
                Type = x.Provider.Type,
                IsPrimary = x.IsPrimary,
                UpdateDate = x.UpdateDate
            }).ToList()
        };
        
        return Result.Success(response);
    }
}