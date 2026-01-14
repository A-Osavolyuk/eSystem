using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserLoginMethodsQuery(Guid UserId) : IRequest<Result>;

public class GetUserLoginMethodsQueryHandler(
    IUserManager userManager,
    IPasskeyManager passkeyManager,
    IPasswordManager passwordManager,
    ITwoFactorManager twoFactorManager,
    IDeviceManager deviceManager,
    ILinkedAccountManager linkedAccountManager,
    IHttpContextAccessor accessor) : IRequestHandler<GetUserLoginMethodsQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly HttpContext _httpContext = accessor.HttpContext!;

    public async Task<Result> Handle(GetUserLoginMethodsQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null || device.IsBlocked || !device.IsTrusted)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidDevice,
                Description = "Invalid device"
            });
        }

        var password = await _passwordManager.GetAsync(user, cancellationToken);
        var linkedAccounts = await _linkedAccountManager.GetAllAsync(user, cancellationToken);
        var passkeys = await _passkeyManager.GetAllAsync(user, cancellationToken);

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
                PasskeyEnabled =
                    await _twoFactorManager.HasMethodAsync(user, TwoFactorMethod.Passkey, cancellationToken),
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
                Passkeys = passkeys.Select(passkey => new UserPasskeyDto()
                    {
                        Id = passkey.Id,
                        CurrentKey = passkey.DeviceId == device.Id,
                        DisplayName = passkey.DisplayName,
                        LastSeenDate = passkey.LastSeenDate,
                        CreateDate = passkey.CreateDate
                    })
                    .ToList()
            }
        };

        return Results.Ok(response);
    }
}