using eSecurity.Core.Common.DTOs;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Credentials.PublicKey;
using eSecurity.Server.Security.Identity.Email;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserVerificationDataQuery(Guid UserId) : IRequest<Result>;

public class GetUserVerificationMethodsQueryHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    IPasskeyManager passkeyManager,
    IEmailManager emailManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserVerificationDataQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IEmailManager _emailManager = emailManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserVerificationDataQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null) return Results.BadRequest("Invalid device.");

        var email = await _emailManager.FindByTypeAsync(user, EmailType.Primary, cancellationToken);
        if (email is null) return Results.BadRequest("Invalid email.");

        var passkey = await _passkeyManager.FindByDeviceAsync(device, cancellationToken);
        var response = new UserVerificationData()
        {
            EmailEnabled = true,
            PasskeyEnabled = passkey is not null,
            AuthenticatorEnabled = user.TwoFactorEnabled,
            PreferredMethod = (user, passkey) switch
            {
                ({ TwoFactorEnabled: true }, null) => VerificationMethod.AuthenticatorApp,
                ({ TwoFactorEnabled: true }, not null) => VerificationMethod.Passkey,
                ({ TwoFactorEnabled: false }, not null) => VerificationMethod.Passkey,
                _ => VerificationMethod.Email
            },
        };

        return Results.Ok(response);
    }
}