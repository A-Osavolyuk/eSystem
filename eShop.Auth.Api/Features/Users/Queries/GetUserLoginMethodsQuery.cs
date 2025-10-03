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
            PasswordData = new PasswordData()
            {
                HasPassword = user.HasPassword(),
                LastChange = user.PasswordChangeDate
            },
            TwoFactorData = new TwoFactorData()
            {
                HasTwoFactor = user.TwoFactorEnabled,
                Providers = user.Providers.Select(x => new UserProviderDto()
                {
                    Id = x.Provider.Id,
                    Type = x.Provider.Type,
                    IsPrimary = x.IsPrimary,
                    UpdateDate = x.UpdateDate
                }).ToList()
            },
            LinkedAccountsData = new LinkedAccountsData()
            {
                HasLinkedAccounts = user.HasLinkedAccount(),
                LinkedAccounts = user.LinkedAccounts.Select(x => new UserOAuthProviderDto()
                {
                    Id = x.Provider.Id,
                    Name = x.Provider.Name,
                    IsAllowed = x.Allowed,
                    DisallowedDate = x.UpdateDate,
                    LinkedDate = x.CreateDate
                }).ToList()
            },
            PasskeysData = new PasskeysData()
            {
                HasPasskeys = user.HasPasskeys(),
                Passkeys = user.Passkeys.Select(x => new UserPasskeyDto()
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName,
                    CreateDate = x.UpdateDate
                }).ToList()
            }
        };

        return Result.Success(response);
    }
}