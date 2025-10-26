using eSystem.Core.Common.Http;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.DTOs;
using eSystem.Core.Security.Authentication.TwoFactor;

namespace eSystem.Auth.Api.Features.Users.Queries;

public record GetUserLoginMethodsQuery(Guid UserId) : IRequest<Result>;

public class GetUserLoginMethodsQueryHandler(
    IUserManager userManager,
    IHttpContextAccessor accessor) : IRequestHandler<GetUserLoginMethodsQuery, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly HttpContext httpContext = accessor.HttpContext!;

    public async Task<Result> Handle(GetUserLoginMethodsQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var userAgent = httpContext.GetUserAgent();
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
                HasLinkedAccounts = user.HasLinkedAccounts(),
                LinkedAccounts = user.LinkedAccounts.Select(linkedAccount => new UserLinkedAccountDto()
                {
                    Id = linkedAccount.Id,
                    Type = linkedAccount.Type,
                    LinkedDate = linkedAccount.CreateDate
                }).ToList()
            },
            PasskeysData = new PasskeysData()
            {
                HasPasskeys = user.HasPasskeys(),
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

        return Result.Success(response);
    }
}