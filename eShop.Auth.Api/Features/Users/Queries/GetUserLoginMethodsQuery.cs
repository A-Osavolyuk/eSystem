using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserLoginMethodsQuery(Guid UserId) : IRequest<Result>;

public class GetUserLoginMethodsQueryHandler(
    IUserManager userManager) : IRequestHandler<GetUserLoginMethodsQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserLoginMethodsQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = new UserLoginMethodsDto()
        {
            HasPassword = user.HasLoginMethod(LoginType.Password),
            HasTwoFactor = user.HasLoginMethod(LoginType.TwoFactor),
            HasLinkedAccounts = user.HasLoginMethod(LoginType.OAuth),
            HasPasskeys = user.HasLoginMethod(LoginType.Password),
        };
        
        return Result.Success(response);
    }
}