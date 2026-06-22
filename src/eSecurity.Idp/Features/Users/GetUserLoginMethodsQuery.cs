using eSecurity.Core.DTOs;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authentication.Password;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Authorization.LinkedAccount;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public record GetUserLoginMethodsQuery() : IRequest<Result>;

public class GetUserLoginMethodsQueryHandler(
    ICurrentUserAccessor currentUserAccessor,
    IPasskeyManager passkeyManager,
    IPasswordManager passwordManager,
    ITwoFactorManager twoFactorManager,
    IDeviceManager deviceManager,
    ILinkedAccountManager linkedAccountManager,
    IHttpContextAccessor accessor) : IRequestHandler<GetUserLoginMethodsQuery, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly HttpContext _httpContext = accessor.HttpContext!;

    public async Task<Result> Handle(GetUserLoginMethodsQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _currentUserAccessor.GetCurrentUserAsync(cancellationToken);
        if (!userResult.Succeeded)
        {
            var error = userResult.GetError();
            return Results.ClientError(ClientErrorCode.Unauthorized, error);
        }

        if (!userResult.TryGetValue(out var user))
        {
            return Results.ClientError(ClientErrorCode.Unauthorized, new Error()
            {
                Code = ErrorCode.Unauthorized,
                Description = "Unauthorized"
            });
        }

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null || device.IsBlocked)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidDevice,
                Description = "Invalid device"
            });
        }

        var password = await _passwordManager.GetAsync(user, cancellationToken);
        var linkedAccounts = await _linkedAccountManager.GetAllAsync(user, cancellationToken);
        var passkeys = await _passkeyManager.GetAllAsync(user, cancellationToken);

        var response = new UserLoginMethodsDto
        {
            PasswordData = new PasswordData
            {
                HasPassword = password is not null,
                LastChangedAt = password?.UpdatedAt
            },
            TwoFactorData = new TwoFactorData
            {
                PreferredMethod = (await _twoFactorManager.GetPreferredAsync(user, cancellationToken))?.Method,
                Enabled = await _twoFactorManager.IsEnabledAsync(user, cancellationToken),
                PasskeyEnabled =
                    await _twoFactorManager.HasMethodAsync(user, TwoFactorMethod.Passkey, cancellationToken),
                SmsEnabled = await _twoFactorManager.HasMethodAsync(user, TwoFactorMethod.Sms, cancellationToken),
                AuthenticatorEnabled = await _twoFactorManager.HasMethodAsync(
                    user, TwoFactorMethod.AuthenticatorApp, cancellationToken),
            },
            LinkedAccountsData = new LinkedAccountsData
            {
                HasLinkedAccounts = linkedAccounts.Count > 0,
                LinkedAccounts = linkedAccounts.Select(linkedAccount => new UserLinkedAccountDto
                {
                    Id = linkedAccount.Id,
                    Type = linkedAccount.Type,
                    LinkedAt = linkedAccount.CreatedAt
                }).ToList()
            },
            PasskeysData = new PasskeysData
            {
                HasPasskeys = await _passkeyManager.HasAsync(user, cancellationToken),
                Passkeys = passkeys.Select(passkey => new UserPasskeyDto
                    {
                        Id = passkey.Id,
                        CurrentKey = passkey.DeviceId == device.Id,
                        DisplayName = passkey.DisplayName,
                        LastSeenAt = passkey.LastSeenDate,
                        CreatedAt = passkey.CreatedAt
                    })
                    .ToList()
            }
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}