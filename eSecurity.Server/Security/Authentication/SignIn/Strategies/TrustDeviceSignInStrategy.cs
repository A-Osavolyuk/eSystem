using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authentication.SignIn.Session;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public sealed class TrustDeviceSignInStrategy(
    ISignInSessionManager signInSessionManager,
    IUserManager userManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    IVerificationManager verificationManager,
    ITwoFactorManager twoFactorManager,
    ISessionManager sessionManager) : ISignInStrategy
{
    private readonly ISignInSessionManager _signInSessionManager = signInSessionManager;
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload is not TrustDeviceSignInPayload trustPayload)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidPayloadType, 
                Description = "Invalid payload type"
            });
        }
        
        var signInSession = await _signInSessionManager.FindByIdAsync(trustPayload.Sid, cancellationToken);
        if (signInSession is null || !signInSession.IsActive) 
            return Results.BadRequest("Invalid sign-in session.");
        
        var user = await _userManager.FindByIdAsync(signInSession.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var userAgent = _httpContext.GetUserAgent()!;
        var ipAddress = _httpContext.GetIpV4()!;
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null || device.IsBlocked)
        {
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidDevice,
                Description = "Invalid device."
            });
        }
        
        var verificationResult = await _verificationManager.VerifyAsync(user, 
            PurposeType.Device, ActionType.Trust, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;
        
        var deviceResult = await _deviceManager.TrustAsync(device, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;
        
        await _sessionManager.CreateAsync(device, cancellationToken);
        return Results.Ok(new SignInResponse()
        {
            UserId = user.Id,
            SessionId = signInSession.Id,
            IsDeviceTrusted = true,
            IsTwoFactorEnabled = await _twoFactorManager.IsEnabledAsync(user, cancellationToken)
        });
    }
}