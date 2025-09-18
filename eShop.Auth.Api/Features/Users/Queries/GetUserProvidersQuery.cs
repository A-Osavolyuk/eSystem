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
            Id = provider.TwoFactorProvider.Id,
            Name = provider.TwoFactorProvider.Name,
            Subscribed = provider.Subscribed,
            UpdateDate = provider.UpdateDate,
            Credential = provider.TwoFactorProvider.Name switch
            {
                ProviderTypes.Email => user.PrimaryEmail?.Email,
                ProviderTypes.Sms => user.PrimaryPhoneNumber?.PhoneNumber,
                _ => null
            }
            
        }).ToList();
        
        return Result.Success(result);
    }
}