using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserLoginMethodsQuery(Guid UserId) : IRequest<Result>;

public class GetUserLoginMethodsQueryHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IPasswordManager passwordManager,
    ITwoFactorManager twoFactorManager,
    ILinkedAccountManager linkedAccountManager,
    IHttpContextAccessor accessor) : IRequestHandler<GetUserLoginMethodsQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly HttpContext _httpContext = accessor.HttpContext!;

    public async Task<Result> Handle(GetUserLoginMethodsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var userAgent = _httpContext.GetUserAgent();
        var password = await _passwordManager.GetAsync(user, cancellationToken);
        var linkedAccounts = await _linkedAccountManager.GetAllAsync(user, cancellationToken);

        var response = new UserLoginMethodsDto()
        {
            PasswordData = new PasswordData()
            {
                HasPassword = password is not null,
                LastChange = password?.UpdateDate
            },
            TwoFactorData = new TwoFactorData()
            {
                PreferredMethod = (await _twoFactorManager.GetPreferredAsync(user, cancellationToken))?.Method,
                Enabled = await _twoFactorManager.IsEnabledAsync(user, cancellationToken),
                PasskeyEnabled = await _twoFactorManager.HasMethodAsync(user, TwoFactorMethod.Passkey, cancellationToken),
                SmsEnabled = await _twoFactorManager.HasMethodAsync(user, TwoFactorMethod.Sms, cancellationToken),
                AuthenticatorEnabled = await _twoFactorManager.HasMethodAsync(
                    user, TwoFactorMethod.AuthenticatorApp, cancellationToken),
            },
            LinkedAccountsData = new LinkedAccountsData()
            {
                HasLinkedAccounts = linkedAccounts.Count > 0,
                LinkedAccounts = linkedAccounts.Select(linkedAccount => new UserLinkedAccountDto()
                {
                    Id = linkedAccount.Id,
                    Type = linkedAccount.Type,
                    LinkedDate = linkedAccount.CreateDate
                }).ToList()
            },
            PasskeysData = new PasskeysData()
            {
                HasPasskeys = await _passkeyManager.HasAsync(user, cancellationToken),
                Passkeys = user.Devices
                    .Where(x => x.Passkey is not null)
                    .Select(device => new UserPasskeyDto()
                    {
                        Id = device.Passkey!.Id,
                        CurrentKey = device.UserAgent!.Equals(userAgent),
                        DisplayName = device.Passkey!.DisplayName,
                        LastSeenDate = device.Passkey!.LastSeenDate,
                        CreateDate = device.Passkey!.CreateDate
                    })
                    .ToList()
            }
        };

        return Results.Ok(response);
    }
}