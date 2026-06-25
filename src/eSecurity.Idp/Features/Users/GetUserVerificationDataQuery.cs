using eSecurity.Core.DTOs;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Security.Authentication.TwoFactor;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Credentials.PublicKey;
using eSecurity.Idp.Security.Identity.Email;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public record GetUserVerificationDataQuery : IRequest<Result>;

public class GetUserVerificationMethodsQueryHandler(
    ICurrentUserAccessor currentUserAccessor,
    IDeviceManager deviceManager,
    IEmailQueryService emailQueryService,
    ISoftwareKeyQueryService softwareKeyQueryService,
    ITwoFactorQueryService twoFactorQueryService,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserVerificationDataQuery, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IEmailQueryService _emailQueryService = emailQueryService;
    private readonly ISoftwareKeyQueryService _softwareKeyQueryService = softwareKeyQueryService;
    private readonly ITwoFactorQueryService _twoFactorQueryService = twoFactorQueryService;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserVerificationDataQuery request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
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

        var softwareKey = await _softwareKeyQueryService.GetByDeviceAsync(device.Id, cancellationToken);
        var twoFactorMethods = await _twoFactorQueryService.ListByUserAsync(user.Id, cancellationToken);
        var response = new UserVerificationData
        {
            EmailEnabled = true,
            SoftwareKeyEnabled = softwareKey is not null,
            AuthenticatorAppEnabled = twoFactorMethods.Any(x => x.Method == TwoFactorMethod.AuthenticatorApp),
            PreferredMethod = (twoFactorMethods.Count > 0, softwareKey) switch
            {
                (true, null) => VerificationMethod.AuthenticatorApp,
                (_, not null) => VerificationMethod.SoftwareKey,
                _ => VerificationMethod.EmailOtp
            },
        };

        return Results.Success(SuccessCodes.Ok, response);
    }
}