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
                Enabled = user.TwoFactorEnabled,
                AuthenticatorEnabled = user.HasTwoFactor(TwoFactorMethod.AuthenticatorApp),
                PasskeyEnabled = user.HasTwoFactor(TwoFactorMethod.Passkey),
                SmsEnabled = user.HasTwoFactor(TwoFactorMethod.Sms),
                PreferredMethod = user.Methods.SingleOrDefault(x => x.Preferred)?.Method,
            },
            LinkedAccountsData = new LinkedAccountsData()
            {
                HasLinkedAccounts = user.HasLinkedAccount(),
                LinkedAccounts = user.LinkedAccounts.Select(linkedAccount => new UserOAuthProviderDto()
                {
                    Id = linkedAccount.Provider.Id,
                    Name = linkedAccount.Provider.Name,
                    IsAllowed = linkedAccount.Allowed,
                    DisallowedDate = linkedAccount.UpdateDate,
                    LinkedDate = linkedAccount.CreateDate
                }).ToList()
            },
            PasskeysData = new PasskeysData()
            {
                HasPasskeys = user.HasPasskeys(),
                Passkeys = user.Devices
                    .Where(x => x.Passkey is not null)
                    .Select(x => x.Passkey!)
                    .Select(passkey => new UserPasskeyDto()
                    {
                        Id = passkey.Id,
                        DisplayName = passkey.DisplayName,
                        LastSeenDate = passkey.LastSeenDate,
                        CreateDate = passkey.UpdateDate
                    }).ToList()
            }
        };

        return Result.Success(response);
    }
}