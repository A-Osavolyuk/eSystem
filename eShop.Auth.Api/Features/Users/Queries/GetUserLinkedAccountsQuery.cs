using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Features.Users.Queries;

public record GetUserLinkedAccountsQuery(Guid UserId) : IRequest<Result>;

public class GetUserLinkedAccountQueryHandler(
    IUserManager userManager) : IRequestHandler<GetUserLinkedAccountsQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserLinkedAccountsQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = user.LinkedAccounts.Select(linkedAccount =>
            new UserOAuthProviderDto()
            {
                Id = linkedAccount.Provider.Id,
                Name = linkedAccount.Provider.Name,
                IsAllowed = linkedAccount.Allowed,
                DisallowedDate = linkedAccount.UpdateDate,
                LinkedDate = linkedAccount.CreateDate
            }).ToList();

        return Result.Success(response);
    }
}