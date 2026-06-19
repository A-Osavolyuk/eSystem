using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSecurity.Core.DTOs;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Identity;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users.Queries;

public record GetUserVerificationDataQuery : IRequest<Result>;

public class GetUserVerificationMethodsQueryHandler(
    ICurrentUserAccessor currentUserAccessor,
    IDeviceManager deviceManager,
    IPasskeyManager passkeyManager,
    IEmailQueryService emailQueryService,
    ITwoFactorManager twoFactorManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserVerificationDataQuery, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IPasskeyManager _passkeyManager = passkeyManager;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserVerificationDataQuery request, CancellationToken cancellationToken)
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
        if (device is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidDevice,
                Description = "Invalid device"
            });
        }

        var email = await _emailQueryService.GetByTypeAsync(user.Id, EmailType.Primary, cancellationToken);
        if (email is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidEmail,
                Description = "Invalid email"
            });
        }

        var passkey = await _passkeyManager.FindByDeviceAsync(device, cancellationToken);
        var twoFactorEnabled = await _twoFactorManager.IsEnabledAsync(user, cancellationToken);
        
        var response = new UserVerificationData
        {
            EmailEnabled = true,
            PasskeyEnabled = passkey is not null,
            AuthenticatorEnabled = await _twoFactorManager.IsEnabledAsync(user, cancellationToken),
            PreferredMethod = (twoFactorEnabled, passkey) switch
            {
                (true, null) => VerificationMethod.AuthenticatorApp,
                (_, not null) => VerificationMethod.Passkey,
                _ => VerificationMethod.EmailOtp
            },
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}