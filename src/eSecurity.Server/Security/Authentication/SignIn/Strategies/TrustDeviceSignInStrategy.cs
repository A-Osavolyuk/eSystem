using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authentication.SignIn.Session;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.SignIn.Session;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;

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
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPayloadType,
                Description = "Invalid payload type"
            });
        }

        var session = await _signInSessionManager.FindByIdAsync(trustPayload.Sid, cancellationToken);
        if (session is null || !session.IsActive || !session.UserId.HasValue)
            return Results.BadRequest("Invalid sign-in session.");

        var user = await _userManager.FindByIdAsync(session.UserId.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var userAgent = _httpContext.GetUserAgent()!;
        var ipAddress = _httpContext.GetIpV4()!;
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null || device.IsBlocked)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidDevice,
                Description = "Invalid device."
            });
        }

        var verificationResult = await _verificationManager.VerifyAsync(user,
            PurposeType.Device, ActionType.Trust, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var deviceResult = await _deviceManager.TrustAsync(device, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;

        session.CompletedSteps.Add(SignInStep.DeviceTrust);
        var remainingSteps = session.RequiredSteps.Except(session.CompletedSteps).ToList();
        if (remainingSteps.Count == 0)
        {
            session.CurrentStep = SignInStep.Complete;
            session.Status = SignInStatus.Completed;
            
            await _sessionManager.CreateAsync(user, cancellationToken);
        }
        else
        {
            session.CurrentStep = remainingSteps.First();
        }
        
        var sessionResult = await _signInSessionManager.UpdateAsync(session, cancellationToken);
        if (!sessionResult.Succeeded) return sessionResult;

        return Results.Ok(new SignInResponse { SessionId = session.Id });
    }
}