using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Verification;
using eSecurity.Server.Security.Authentication.TwoFactor.Secret;
using eSecurity.Server.Security.Authorization;
using eSecurity.Server.Security.Authorization.Verification;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Identity.Claims;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Features.TwoFactor.Commands;

public record ReconfigureAuthenticatorCommand(ReconfigureAuthenticatorRequest Request) : IRequest<Result>;

public class ReconfigureAuthenticatorCommandHandler(
    IUserManager userManager,
    ISecretManager secretManager,
    IVerificationManager verificationManager,
    IDataProtectionProvider protectionProvider,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<ReconfigureAuthenticatorCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ISecretManager _secretManager = secretManager;
    private readonly IVerificationManager _verificationManager = verificationManager;
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(ReconfigureAuthenticatorCommand request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid request");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var verification = await _verificationManager.FindByIdAsync(request.Request.VerificationId, cancellationToken);
        if (verification?.Status is not VerificationStatus.Approved) 
            return Results.BadRequest("Unverified request.");

        var verificationResult = await _verificationManager.ConsumeAsync(verification, cancellationToken);
        if (!verificationResult.Succeeded) return verificationResult;

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        var protectedSecret = protector.Protect(request.Request.Secret);
        var userSecret = await _secretManager.GetAsync(user, cancellationToken);
        if (userSecret is null) return Results.NotFound("Secret not found");
        
        userSecret.ProtectedSecret = protectedSecret;

        var result = await _secretManager.UpdateAsync(userSecret, cancellationToken);
        return result;
    }
}