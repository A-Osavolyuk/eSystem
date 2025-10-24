using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.DTOs;

namespace eSystem.Auth.Api.Features.Users.Queries;

public record GetUserTwoFactorMethodsQuery(Guid Id) : IRequest<Result>;

public class GetUserProvidersQueryHandler(IUserManager userManager) : IRequestHandler<GetUserTwoFactorMethodsQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserTwoFactorMethodsQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Id}.");

        var providers = user.Methods.ToList();
        var result = providers.Select(provider => new UserTwoFactorMethod()
        {
            Method = provider.Method,
            Preferred = provider.Preferred,
            UpdateDate = provider.UpdateDate,
        }).ToList();
        
        return Result.Success(result);
    }
}